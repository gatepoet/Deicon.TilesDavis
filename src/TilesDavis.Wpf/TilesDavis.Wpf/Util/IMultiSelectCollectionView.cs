using System.Windows.Controls.Primitives;

namespace TilesDavis.Wpf.Util
{
    public interface IMultiSelectCollectionView
    {
        void AddControl(Selector selector);
        void RemoveControl(Selector selector);
    }
}