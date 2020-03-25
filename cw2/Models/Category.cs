using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cwiczenie_2.models
{
	[XmlRoot("uczelnia")]
	public class Category
	{
		public class Group
		{
			private List<Item> students;
			private List<Item> studies;

			private string gDate;
			private string gAuthor;
			

			public Group(string gDate, string gAuthor, List<Item> students, List<Item> studies)
			{
				this.gDate=gDate;
				this.gAuthor=gAuthor;

				this.students=students;
				this.studies=studies;
			}

			[XmlIgnore, JsonPropertyName("createdAt")]
			public string GDate=>this.gDate;

			[XmlIgnore, JsonPropertyName("author")]
			public string GAuthor=>this.gAuthor;

			[XmlIgnore, JsonPropertyName("studenci")]
			public List<Item> StudentList=>this.students;

			[XmlIgnore, JsonPropertyName("activeStudies")]
			public List<Item> StudiesList=>this.studies;
		}

		[XmlAttribute("createdAt"), JsonIgnore]
        public string Date{get; set;}

		[XmlAttribute("author"), JsonIgnore]
        public string Author{get; set;}

		[XmlArray("studenci"), XmlArrayItem("student")]
        public List<Item> students=new List<Item>();

		[XmlArray("activeStudies"), XmlArrayItem("studies")]
		public List<Item> studies=new List<Item>();

		[XmlIgnore, JsonPropertyName("uczelnia")]
		public Group Colleage=>new Group(this.Date, this.Author, this.students, this.studies);
	}
}