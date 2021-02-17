// GercStudio
// © 2018-2020

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEditorInternal;
using Image = UnityEngine.UI.Image;

namespace GercStudio.USK.Scripts
{
	public class CreateWeaponWindow : EditorWindow
	{
		public GameObject WeaponModel;

		public List<WeaponsHelper.TypeOfAttack> Attacks = new List<WeaponsHelper.TypeOfAttack> {WeaponsHelper.TypeOfAttack.Bullets};

		private ReorderableList _attacks;
		
		private float startVal;
		private float progress;

		private bool WeaponAdded;
		private bool hasCreated;
		private bool startCreation;
		private bool characterError;
		private bool CharacterAdded;

		private Font font;

		private GUIStyle LabelStyle;

		private Vector2 scrollPos;

		[MenuItem("Tools/USK/Create/Weapon")]
		public static void ShowWindow()
		{
			GetWindow(typeof(CreateWeaponWindow), true, "", true).ShowUtility();
		}

		void OnEnable()
		{
			_attacks = new ReorderableList(Attacks, typeof(WeaponsHelper.TypeOfAttack), false, true, true, true)
            {
	            
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Attacks:");
                },

                onAddCallback = items =>
                {
	                Attacks.Add(WeaponsHelper.TypeOfAttack.Bullets);
                },
                
                onRemoveCallback = items =>
                {
                    if(Attacks.Count == 1)
                        return;

                    Attacks.Remove(Attacks[items.index]);
                },
                
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
	              Attacks[index] = (WeaponsHelper.TypeOfAttack)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), Attacks[index]);
	                
                }
            };
			EditorApplication.update += Update;
		}

		void OnDisable()
		{
			EditorApplication.update -= Update;
		}

		private void Awake()
		{
			if(!font)
				font = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Textures & Materials/Other/Font/hiragino.otf", typeof(Font)) as Font;
			
			if (LabelStyle != null) return;

			LabelStyle = new GUIStyle
			{
				normal = {textColor = Color.black},
				fontStyle = FontStyle.Bold,
				fontSize = 12,
				alignment = TextAnchor.MiddleCenter
			};
		}

		void Update()
		{
			
			if (WeaponModel)
			{
				if (!WeaponAdded)
				{
					WeaponAdded = true;
				}
			}

			if (startCreation & progress > 1.1f)
			{
				ManageParent();
				AddScripts();
				SetVariabales();
				CreateObjects();
				SaveWeaponToPrefab();

				hasCreated = true;
				startVal = (float) EditorApplication.timeSinceStartup;

				startCreation = false;
			}

			if (hasCreated)
			{
				if (progress > 10)
				{
					hasCreated = false;

					WeaponModel = null;
				}
			}

		}

		private void OnGUI()
		{
			scrollPos =
				EditorGUILayout.BeginScrollView(scrollPos, false, false, GUILayout.Width(position.width),
					GUILayout.Height(position.height));


			EditorGUILayout.Space();
			GUILayout.Label("Create Weapon", LabelStyle);
			EditorGUILayout.Space();
			if (hasCreated)
			{
				GUIStyle style = new GUIStyle
				{
					normal = {textColor = Color.green}, fontStyle = FontStyle.Bold, fontSize = 10, alignment = TextAnchor.MiddleCenter
				};
				EditorGUILayout.LabelField("Weapon has been created", style);


				EditorGUILayout.HelpBox("1) Add other parameters (like animations, sounds, etc) to the [WeaponController] script." + "\n\n" +
				                        "2) Open the Adjustment scene [Tools -> USK -> Adjust] to adjust the weapon and hands positions and rotations.", MessageType.Info);


				EditorGUILayout.Space();
			}

			EditorGUILayout.BeginVertical("HelpBox");
			WeaponModel = (GameObject) EditorGUILayout.ObjectField("Weapon Model", WeaponModel, typeof(GameObject), true);
			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();

			_attacks.DoList(new Rect(3, GUILayoutUtility.GetLastRect().max.y + 10, position.width - 6, _attacks.GetHeight()));
			EditorGUILayout.Space();

			if (WeaponModel)
			{
				if (startCreation)
				{
					if (progress < 0.3f)
						EditorGUI.ProgressBar(new Rect(3, GUILayoutUtility.GetLastRect().max.y + _attacks.GetHeight() + 10, position.width - 6, 20), progress / 1, "Creation.");
					else if (progress > 0.3f && progress < 0.6f)
						EditorGUI.ProgressBar(new Rect(3, GUILayoutUtility.GetLastRect().max.y + _attacks.GetHeight() + 10, position.width - 6, 20), progress / 1, "Creation..");
					else if (progress > 0.6f)
						EditorGUI.ProgressBar(new Rect(3, GUILayoutUtility.GetLastRect().max.y + _attacks.GetHeight() + 10, position.width - 6, 20), progress / 1, "Creation...");
				}
			}
			else
			{
				WeaponAdded = false;
			}


			EditorGUI.BeginDisabledGroup(!WeaponModel); // || !parameters);

			if (!startCreation & !hasCreated)
				if (GUI.Button(new Rect(3, GUILayoutUtility.GetLastRect().max.y + _attacks.GetHeight() + 10, position.width - 6, 20), "Create"))
				{
					startVal = (float) EditorApplication.timeSinceStartup;
					startCreation = true;
				}

			EditorGUI.EndDisabledGroup();

			EditorGUILayout.EndScrollView();

			progress = (float) (EditorApplication.timeSinceStartup - startVal);
		}

		void OnInspectorUpdate()
		{
			Repaint();
		}

		void CreateObjects()
		{
			WeaponController controller = WeaponModel.GetComponent<WeaponController>();

			foreach (var attack in controller.Attacks)
			{
				if (attack.AttackType == WeaponsHelper.TypeOfAttack.Bullets && !attack.ShellPoint)
				{
					attack.ShellPoint = controller.Attacks.Any(_attack => _attack.ShellPoint)
						? controller.Attacks.Find(_attack => _attack.ShellPoint).ShellPoint
						: Helper.NewPoint(controller.gameObject, "Shell Spawn Point");
				}

				if (attack.AttackType != WeaponsHelper.TypeOfAttack.Melee && attack.AttackType != WeaponsHelper.TypeOfAttack.Grenade && !attack.AttackSpawnPoint)
				{
					attack.AttackSpawnPoint = controller.Attacks.Any(_attack => _attack.AttackSpawnPoint)
						? controller.Attacks.Find(_attack => _attack.AttackSpawnPoint).AttackSpawnPoint
						: Helper.NewPoint(controller.gameObject, "Attack Point");
				}


				if (attack.AttackType == WeaponsHelper.TypeOfAttack.Melee)
				{
					attack.AttackCollider = controller.Attacks.Any(_attack => _attack.AttackCollider && _attack.AttackType == WeaponsHelper.TypeOfAttack.Melee)
						? controller.Attacks.Find(_attack => _attack.AttackCollider && _attack.AttackType == WeaponsHelper.TypeOfAttack.Melee).AttackCollider
						: Helper.NewCollider("Melee Collider", "Melee Collider", WeaponModel.transform);
				}

				if (attack.AttackType == WeaponsHelper.TypeOfAttack.Flame)
				{
					attack.AttackCollider = controller.Attacks.Any(_attack => _attack.AttackCollider && _attack.AttackType == WeaponsHelper.TypeOfAttack.Flame)
						? controller.Attacks.Find(_attack => _attack.AttackCollider && _attack.AttackType == WeaponsHelper.TypeOfAttack.Flame).AttackCollider
						: Helper.NewCollider("Fire Collider", "Fire", WeaponModel.transform);

				}
			}
			
			if (!controller.inspectorCanvas)
			{
				controller.inspectorCanvas = Helper.NewCanvas("Canvas", new Vector2(1920, 1080), WeaponModel.transform);
			}
			
			var parts = CharacterHelper.CreateCrosshair(controller.inspectorCanvas.transform);

			controller.upPart = parts[1].GetComponent<Image>();
			controller.downPart = parts[2].GetComponent<Image>();
			controller.rightPart = parts[3].GetComponent<Image>();
			controller.leftPart = parts[4].GetComponent<Image>();
			controller.middlePart = parts[5].GetComponent<Image>();
		}

		void AddScripts()
		{
			if (!WeaponModel.GetComponent<WeaponController>())
				WeaponModel.AddComponent<WeaponController>();

			if (!WeaponModel.GetComponent<Rigidbody>()) return;
			WeaponModel.GetComponent<Rigidbody>().useGravity = false;
			WeaponModel.GetComponent<Rigidbody>().isKinematic = true;
		}

		void ManageParent()
		{
			var name = WeaponModel.name;
			WeaponModel = Instantiate(WeaponModel, Vector3.zero, Quaternion.Euler(Vector3.zero));
			
			if (WeaponModel.GetComponent<Animator>())
				DestroyImmediate(WeaponModel.GetComponent<Animator>());

			var parent = new GameObject(name).transform;
			parent.parent = WeaponModel.transform;
			parent.localPosition = Vector3.zero;
			parent.localRotation = Quaternion.Euler(Vector3.zero);
			parent.parent = null;
			WeaponModel.transform.parent = parent;
			WeaponModel.name = "Render";
			WeaponModel = parent.gameObject;
		}

		void SaveWeaponToPrefab()
		{
			if (!AssetDatabase.IsValidFolder("Assets/Universal Shooter Kit/Prefabs/Weapons/"))
			{
				Directory.CreateDirectory("Assets/Universal Shooter Kit/Prefabs/Weapons/");
			}
			
			var index = 0;
			while(AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Prefabs/Weapons/" + WeaponModel.name + " " + index + ".prefab", typeof(GameObject)) != null)
			{
				index++;
			}

#if !UNITY_2018_3_OR_NEWER
			var prefab = PrefabUtility.CreateEmptyPrefab("Assets/Universal Shooter Kit/Prefabs/Weapons/" + WeaponModel.name + " " + index + ".prefab");
			PrefabUtility.ReplacePrefab(WeaponModel, prefab, ReplacePrefabOptions.ConnectToPrefab);
#else
			PrefabUtility.SaveAsPrefabAsset(WeaponModel, "Assets/Universal Shooter Kit/Prefabs/Weapons/" + WeaponModel.name + " " + index + ".prefab");
#endif

			EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Prefabs/Weapons/" + WeaponModel.name + " " + index + ".prefab",
				typeof(GameObject)));

			DestroyImmediate(WeaponModel);
			
		}

		void SetVariabales()
		{
			var weaponController = WeaponModel.GetComponent<WeaponController>();

			weaponController.Attacks.Clear();
			weaponController.attacksNames.Clear();
			
			if (Attacks.All(attack => attack != WeaponsHelper.TypeOfAttack.Grenade))
			{
				for (var i = 0; i < Attacks.Count; i++)
				{
					weaponController.Attacks.Add(new WeaponsHelper.Attack {AttackType = Attacks[i]});
					weaponController.attacksNames.Add("Attack " + i);
				}
			}
			else
			{
				weaponController.Attacks.Add(new WeaponsHelper.Attack{AttackType = WeaponsHelper.TypeOfAttack.Grenade});
				weaponController.attacksNames.Add("Grenade");
			}

			weaponController.projectSettings = AssetDatabase.LoadAssetAtPath("Assets/Universal Shooter Kit/Tools/!Settings/Input.asset", typeof(ProjectSettings)) as ProjectSettings;
		}
	}
}
