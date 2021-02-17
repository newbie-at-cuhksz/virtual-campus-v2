using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
namespace VoxelImporter.grendgine_collada
{
	[System.SerializableAttribute()]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
	[System.Xml.Serialization.XmlRootAttribute(ElementName="library_effects", Namespace="http://www.collada.org/2005/11/COLLADASchema", IsNullable=true)]
	public partial class Grendgine_Collada_Library_Effects
	{
		[XmlAttribute("id")]
		public string ID;
		
		[XmlAttribute("name")]
		public string Name;		
		
		
		[XmlElement(ElementName = "asset")]
		public Grendgine_Collada_Asset Asset;

        [XmlElement(ElementName = "effect")]
        public Grendgine_Collada_Effect[] Effect;

        [XmlElement(ElementName = "extra")]
		public Grendgine_Collada_Extra[] Extra;				
		
	}
}

//check done