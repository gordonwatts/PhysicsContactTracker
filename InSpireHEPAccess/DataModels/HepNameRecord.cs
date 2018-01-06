using Newtonsoft.Json;
using System;
using J = Newtonsoft.Json.JsonPropertyAttribute;


namespace InSpireHEPAccess.DataModels
{
    // Below code was auto-generated from a hepnames json response, from https://app.quicktype.io/#r=json2csharp
    // To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
    //
    //    using InSpireHEPAccess.DataModels;
    //
    //    var data = Welcome.FromJson(jsonString);

    namespace InSpireHEPAccess.DataModels
    {
        public partial class Welcome
        {
            [J("files")] public object[] Files { get; set; }
            [J("cataloguer_info")] public CataloguerInfo[] CataloguerInfo { get; set; }
            [J("FIXME_OAI")] public FixmeOai FixmeOai { get; set; }
            [J("number_of_citations")] public long NumberOfCitations { get; set; }
            [J("collection")] public Collection[] Collection { get; set; }
            [J("accelerator_experiment")] public AcceleratorExperiment[] AcceleratorExperiment { get; set; }
            [J("number_of_comments")] public long NumberOfComments { get; set; }
            [J("system_control_number")] public SystemControlNumber[] SystemControlNumber { get; set; }
            [J("creation_date")] public string CreationDate { get; set; }
            [J("system_number")] public SystemNumber SystemNumber { get; set; }
            [J("url")] public Url[] Url { get; set; }
            [J("number_of_reviews")] public long NumberOfReviews { get; set; }
            [J("recid")] public long Recid { get; set; }
            [J("persistent_identifiers_keys")] public string[] PersistentIdentifiersKeys { get; set; }
            [J("source_data_found")] public SourceDataFound[] SourceDataFound { get; set; }
            [J("authors")] public Author[] Authors { get; set; }
            [J("version_id")] public string VersionId { get; set; }
            [J("subject")] public Subject Subject { get; set; }
            [J("filetypes")] public object[] Filetypes { get; set; }
            [J("number_of_authors")] public long NumberOfAuthors { get; set; }
        }

        public partial class Url
        {
            [J("url")] public string PurpleUrl { get; set; }
            [J("description")] public string Description { get; set; }
        }

        public partial class SystemNumber
        {
            [J("value")] public string Value { get; set; }
        }

        public partial class SystemControlNumber
        {
            [J("institute")] public string Institute { get; set; }
            [J("value")] public string Value { get; set; }
        }

        public partial class Subject
        {
            [J("source")] public string Source { get; set; }
            [J("term")] public string Term { get; set; }
        }

        public partial class SourceDataFound
        {
            [J("source_citation")] public string SourceCitation { get; set; }
        }

        public partial class FixmeOai
        {
            [J("set")] public string Set { get; set; }
            [J("id")] public string Id { get; set; }
        }

        public partial class Collection
        {
            [J("primary")] public string Primary { get; set; }
        }

        public partial class CataloguerInfo
        {
            [J("creation_date")] public DateTime? CreationDate { get; set; }
            [J("modification_date")] public DateTime? ModificationDate { get; set; }
        }

        public partial class Author
        {
            [J("first_name")] public string FirstName { get; set; }
            [J("last_name")] public string LastName { get; set; }
            [J("full_name")] public string FullName { get; set; }
        }

        public partial class AcceleratorExperiment
        {
            [J("experiment")] public string Experiment { get; set; }
        }

        public partial class Welcome
        {
            public static Welcome[] FromJson(string json) => JsonConvert.DeserializeObject<Welcome[]>(json, Converter.Settings);
        }

        public static class Serialize
        {
            public static string ToJson(this Welcome[] self) => JsonConvert.SerializeObject(self, Converter.Settings);
        }

        public class Converter
        {
            public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None,
            };
        }
    }
}

