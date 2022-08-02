
namespace Negi0109.HistoryViewer.Models
{
    /// <summary>
    /// UnityYamlドキュメントとその他ドキュメントで構成される情報を持つオブジェクト
    /// </summary>
    /// <remarks>
    /// GameObjectに対するコンポーネントなど複数ドキュメントにまたがるものをここで解決する
    /// </remarks>
    public class UnityYamlDocumentWithExtra
    {
        public readonly UnityYamlDocument document;

        public UnityYamlDocumentWithExtra(UnityYamlDocument document)
        {
            this.document = document;
        }
    }
}
