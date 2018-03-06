using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphMed_Beta
{
    public sealed class CurrentConfig
    {
        /// <summary>
        /// Singleton - only one object of this type i allowed. Multi thread safe
        /// </summary>
        private CurrentConfig()
        {
            destPath = new string[4];
        }

        /// <summary>
        /// Call CurrentConfig Singleton class
        /// </summary>
        public static CurrentConfig Instance { get { return Nested.instance; } }

        private class Nested
        {
            static Nested() { }
            internal static readonly CurrentConfig instance = new CurrentConfig();
        }
        public string GraphDBUser { get; set; }
        public string GraphDBPassword { get; set; }
        public string GraphDBUri { get; set; }
        public string SnomedVersion { get; set; }

        public string[] destPath { get; set; }

        public string snomedImportPath;
        public string SnomedImportPath
        {
            get { return snomedImportPath; }
            set
            {
                snomedImportPath = value;
                Descriptions = String.Concat(value, @"\SnomedCT_InternationalRF2_PRODUCTION_20170731T150000Z\Snapshot\Terminology\sct2_Description_Snapshot-en_INT_", SnomedVersion, ".txt");
                Concepts = String.Concat(value, @"\SnomedCT_InternationalRF2_PRODUCTION_20170731T150000Z\Snapshot\Terminology\sct2_Concept_Snapshot_INT_", SnomedVersion, ".txt");
                Relationships = String.Concat(value, @"\SnomedCT_InternationalRF2_PRODUCTION_20170731T150000Z\Snapshot\Terminology\sct2_Relationship_Snapshot_INT_", SnomedVersion, ".txt");
                Refset = String.Concat(value, @"\SnomedCT_InternationalRF2_PRODUCTION_20170731T150000Z\Snapshot\\Refset\Language\der2_cRefset_LanguageSnapshot-en_INT_", SnomedVersion, ".txt");
            }
        }
        public string Descriptions { get; set; }
        public string Concepts { get; set; }
        public string Relationships { get; set; }
        public string Refset { get; set; }
        public string TargetPath { get; set; }
        public string ImportPath { get; set; }
        public string FullDescription { get; set; }
        public string FullConcept { get; set; }
        public string FullRefset { get; set; }
        public string FullRelationship { get; set; }
    }
}
