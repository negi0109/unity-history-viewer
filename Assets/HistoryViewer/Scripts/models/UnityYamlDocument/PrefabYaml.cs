namespace Negi0109.HistoryViewer.Models
{
    public class PrefabYaml
    {
        public readonly ulong correspondingSourceObjectId;

        public PrefabYaml(UnityYamlDocument doc)
        {
            // TODO リファクタリング
            var lines = doc.content.Split('\n');

            // m_Modification.m_Modifications[0].target.fileID
            // 1つ目に格納されているのは元のGameObjectとの差異のはず
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var indent = YamlUtils.GetIndentSize(line);
                var key = YamlUtils.GetInlineKey(line, indent);
                if (string.IsNullOrEmpty(key)) continue;

                if (key.Equals("m_Modifications"))
                {
                    i++;
                    var line2 = lines[i];
                    var indent2 = YamlUtils.GetIndentSize(line2);
                    var key2 = YamlUtils.GetInlineKey(line2);
                    if (key2.Equals("target"))
                    {
                        var blockValue = YamlUtils.GetInlineValue(line2, key2, indent2);
                        correspondingSourceObjectId = ulong.Parse(YamlUtils.GetBlockValue(blockValue, "fileID"));

                        break;
                    }
                }
            }
        }
    }
}
