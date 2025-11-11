using System.Collections.ObjectModel;

namespace otoface
{
    public class Group
    {
        public string GroupName { get; set; }
        public string Key { get; set; }
        public string FadeFrame { get; set; }
        public ObservableCollection<Bone> Bones { get; set; }
    }

    public class Bone
    {
        public string BoneName { get; set; }
        public string Value { get; set; }
        public string Parts { get; set; }
    }

}
