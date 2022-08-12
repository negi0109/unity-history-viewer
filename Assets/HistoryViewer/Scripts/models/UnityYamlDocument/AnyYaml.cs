using System;

namespace Negi0109.HistoryViewer.Models
{
    public class AnyYaml
    {
        public bool IsBelongsToGameObject { get => gameObjectId != 0; }
        public readonly string name;
        public readonly ulong gameObjectId;
        public readonly string guid;

        public AnyYaml(UnityYamlDocument doc)
        {
            var lines = doc.content.Split("\n");
            name = string.Intern(lines[1][..^1]);

            foreach (var line in lines)
            {
                if (YamlUtils.IsKey(line, "m_GameObject"))
                {
                    var fileID = YamlUtils.GetBlockValue(YamlUtils.GetInlineValue(line, "m_GameObject"), "fileID");
                    if (ulong.TryParse(fileID, out ulong id)) gameObjectId = id;
                    else throw new FormatException();
                }
                if (doc.type == 114 && YamlUtils.IsKey(line, "m_Script"))
                {
                    guid = YamlUtils.GetBlockValue(YamlUtils.GetInlineValue(line, "m_Script"), "guid");
                }
            }
        }
    }
}
