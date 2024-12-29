using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using TrarsUI.Shared.Interfaces.UIComponents;
using vNekoChatUI.A.MVVM.Models;

namespace vNekoChatUI.A.MVVM.ViewModels
{
    partial class PCreatorVM : ObservableObject, IContentVM
    {
        public string Title { get; set; } = "PCreator";

        [ObservableProperty]
        private ContactModel bot;
    }
}

namespace asdasd
{
    public class Appearance
    {
        [JsonPropertyName("general")]
        public string General { get; set; }

        [JsonPropertyName("unique")]
        public string Unique { get; set; }
    }

    public class Attire
    {
        [JsonPropertyName("default")]
        public string Default { get; set; }

        [JsonPropertyName("intimate")]
        public string Intimate { get; set; }
    }

    public class Character
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("age")]
        public string Age { get; set; }

        [JsonPropertyName("race")]
        public string Race { get; set; }

        [JsonPropertyName("appearance")]
        public Appearance Appearance { get; set; }

        [JsonPropertyName("physique")]
        public Physique Physique { get; set; }

        [JsonPropertyName("attire")]
        public Attire Attire { get; set; }

        [JsonPropertyName("voice")]
        public string Voice { get; set; }
    }

    public class Personality
    {
        [JsonPropertyName("general")]
        public string General { get; set; }

        [JsonPropertyName("sexual")]
        public Sexual Sexual { get; set; }
    }

    public class Physiology
    {
        [JsonPropertyName("entrance")]
        public string Entrance { get; set; }

        [JsonPropertyName("passage")]
        public string Passage { get; set; }

        [JsonPropertyName("depth")]
        public string Depth { get; set; }

        [JsonPropertyName("climax")]
        public string Climax { get; set; }
    }

    public class Physique
    {
        [JsonPropertyName("height")]
        public string Height { get; set; }

        [JsonPropertyName("weight")]
        public string Weight { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

    public class Reactions
    {
        [JsonPropertyName("physical")]
        public List<object> Physical { get; set; }

        [JsonPropertyName("emotional")]
        public List<object> Emotional { get; set; }
    }

    public class Relationship
    {
        [JsonPropertyName("with_ryu")]
        public string WithRyu { get; set; }
    }

    public class Root
    {
        [JsonPropertyName("character")]
        public Character Character { get; set; }

        [JsonPropertyName("personality")]
        public Personality Personality { get; set; }

        [JsonPropertyName("physiology")]
        public Physiology Physiology { get; set; }

        [JsonPropertyName("relationship")]
        public Relationship Relationship { get; set; }

        [JsonPropertyName("miscellaneous")]
        public string Miscellaneous { get; set; }
    }

    public class Sexual
    {
        [JsonPropertyName("preferences")]
        public List<object> Preferences { get; set; }

        [JsonPropertyName("reactions")]
        public Reactions Reactions { get; set; }
    }
}