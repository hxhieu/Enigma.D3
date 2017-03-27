using Mitmeo.Common.Utils;
using Mitmeo.D3.App.Commands;
using PropertyChanged;
using System.Windows.Input;

namespace Mitmeo.D3.App.ViewModels
{
    [ImplementPropertyChanged]
    public abstract class SaveableViewModel<TSave> : ViewModelBase
    {
        protected string SaveFileName { get; private set; }

        public abstract TSave Configuration { get; set; }

        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(
                        action => true,
                        action =>
                        {
                            Save();
                        }
                    );
                }

                return _saveCommand;
            }
        }

        private ICommand _loadCommand;
        public ICommand LoadCommand
        {
            get
            {
                if (_loadCommand == null)
                {
                    _loadCommand = new RelayCommand(
                        action => true,
                        action =>
                        {
                            Load();
                        }
                    );
                }

                return _loadCommand;
            }
        }

        public SaveableViewModel(string saveFileName)
        {
            SaveFileName = saveFileName;
            Load();
        }

        public virtual void Save()
        {
            new Serialiser().WriteToBinaryFile(SaveFileName, Configuration);
        }

        public virtual void Load()
        {
            Configuration = new Serialiser().ReadFromBinaryFile<TSave>(SaveFileName);
            Enabled = false;
        }
    }
}
