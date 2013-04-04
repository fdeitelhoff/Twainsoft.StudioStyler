using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Windows.Data;
using Twainsoft.StudioStyler.Services.StudioStyles.Model;

namespace Twainsoft.StudioStyler.VSPackage.GUI
{
  public class SchemesCollectionView : CollectionView
    {
      private readonly IEnumerable<Scheme> _innerList;
        private readonly int _itemsPerPage;

        private int _currentPage = 1;

        public SchemesCollectionView(IEnumerable<Scheme> innerList, int itemsPerPage)
            : base(innerList)
        {
            this._innerList = innerList;
            this._itemsPerPage = itemsPerPage;
        }

        public override int Count
        {
            get { return this._itemsPerPage; }
        }

        public int CurrentPage
        {
            get { return this._currentPage; }
            set
            {
                _currentPage = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentPage"));
                OnPropertyChanged(new PropertyChangedEventArgs("FirstItemNumber"));
                OnPropertyChanged(new PropertyChangedEventArgs("LastItemNumber"));
            }
        }

        public int ItemsPerPage { get { return this._itemsPerPage; } }

        public int PageCount
        {
            get 
            { 
                return (this._innerList.Count() + this._itemsPerPage - 1) 
                    / this._itemsPerPage; 
            }
        }

        public int LastItemNumber
        {
            get
            {
                var end = this._currentPage * this._itemsPerPage - 1;
                end = (end > this._innerList.Count()) ? this._innerList.Count() : end;

                return end + 1;
            }
        }

        public int StartIndex
        {
            get { return (this._currentPage - 1) * this._itemsPerPage; }
        }

        public int FirstItemNumber
        {
            get { return ((this._currentPage - 1) * this._itemsPerPage) + 1; }
        }

        public override object GetItemAt(int index)
        {
            var offset = index % (this._itemsPerPage);
            var list = _innerList as IList<Scheme>;

            var position = this.StartIndex + offset;

            if (position >= list.Count)
            {
                position = list.Count - 1;
            }

            return list[position];
        }

        public void MoveToNextPage()
        {
            if (CurrentPage < this.PageCount)
            {
                CurrentPage += 1;
            }
            Refresh();
        }

        public void MoveToPreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage -= 1;
            }
            Refresh();
        }

      public void MoveToFirstPage()
      {
          CurrentPage = 1;
          Refresh();
      }

      public void MoveToLastPage()
      {
          CurrentPage = PageCount;
          Refresh();
      }
    }
}
