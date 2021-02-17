using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
namespace VoxelImporter.grendgine_collada
{
	[System.SerializableAttribute()]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
	[System.Xml.Serialization.XmlRootAttribute(ElementName="instance_effect", Namespace="http://www.collada.org/2005/11/COLLADASchema", IsNullable=true)]
	public partial class Grendgine_Collada_Instance_Effect
	{
		[XmlAttribute("sid")]
		public string sID;
		
		[XmlAttribute("name")]
		public string Name;		
		
		[XmlAttribute("url")]
		public string URL;

        [XmlElement(ElementName = "technique_hint")]
        public Grendgine_Collada_Technique_Hint[] Technique_Hint;

        [XmlElement(ElementName = "setparam")]
		public Grendgine_Collada_Set_Param[] Set_Param;
		
	    [XmlElement(ElementName = "extra")]
		public Grendgine_Collada_Extra[] Extra;				
		
	}
}

//check done