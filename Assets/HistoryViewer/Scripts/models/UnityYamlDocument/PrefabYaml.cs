namespace Negi0109.HistoryViewer.Models
{
    public class PrefabYaml
    {
        public readonly ulong correspondingSourceObjectId;

        public PrefabYaml(UnityYamlDocument doc)
        {
            // TODO リファクタリング
            var lines = doc.content.Split('\n');

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var indent = YamlUtils.GetIndentSize(line);
                var key = YamlUtils.GetInlineKey(line, indent);
                if (string.IsNullOrEmpty(key)) continue;

                if (key.Equals("m_Modification"))
                {
                    i++;
                    for (; i < lines.Length; i++)
                    {
                        var line1 = lines[i];
                        var indent1 = YamlUtils.GetIndentSize(line1);
                        if (indent1 <= indent)
                        {
                            i--;
                            break;
                        }
                        var key1 = YamlUtils.GetInlineKey(line1);

                        if (key1.Equals("m_Modifications"))
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
    }
}
