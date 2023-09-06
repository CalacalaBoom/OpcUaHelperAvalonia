using app.Core.Models;
using Avalonia.Controls.Notifications;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace app.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Binding
        private string _Address = string.Empty;
        public string Address
        {
            get => _Address;
            set => this.RaiseAndSetIfChanged(ref _Address, value);
        }

        private bool _isFastAccess;
        public bool isFastAccess
        {
            get => _isFastAccess;
            set => this.RaiseAndSetIfChanged(ref _isFastAccess, value);
        }

        private ObservableCollection<Node> _Nodes = new ObservableCollection<Node>();
        public ObservableCollection<Node> Nodes
        {
            get => _Nodes;
            set => this.RaiseAndSetIfChanged(ref _Nodes, value);
        }

        private string _NodeAddress = string.Empty;
        public string NodeAddress
        {
            get => _NodeAddress;
            set => this.RaiseAndSetIfChanged(ref _NodeAddress, value);
        }

        private ObservableCollection<VariableData> _VariableList = new ObservableCollection<VariableData>();
        public ObservableCollection<VariableData> VariableList
        {
            get => _VariableList;
            set => this.RaiseAndSetIfChanged(ref _VariableList, value);
        }

        private string _Tip = string.Empty;
        public string Tip
        {
            get => _Tip;
            set => this.RaiseAndSetIfChanged(ref _Tip, value);
        }
        #endregion

        public MainViewModel()
        {
            Address = "test";
            isFastAccess = true;
            NodeAddress = "test";
            Tip = "Opc Status: [正常] 2023-09-05 17:12:39 Connected [opc.tcp://172.16.36.3/]";

            for (int i = 0; i < 10; i++)
            {
                VariableList.Add(new VariableData()
                {
                    Name = i.ToString(),
                    Value = i.ToString(),
                    Type = i.ToString(),
                    AccessLevel = i.ToString(),
                    Description = i.ToString()
                });
            }

            Nodes.Add(new Node()
            {
                Title = "Sub1",
                SubNodes = new ObservableCollection<Node>()
                {
                    new Node()
                    {
                        Title="Sub2",
                        SubNodes=new ObservableCollection<Node>()
                        {
                            new Node()
                            {
                                Title="1"
                            },
                            new Node()
                            {
                                Title="2"
                            },
                            new Node()
                            {
                                Title ="3"
                            }
                        }
                    }
                }
            });

            Nodes.Add(new Node()
            {
                Title = "Sub1",
                SubNodes = new ObservableCollection<Node>()
                {
                    new Node()
                    {
                        Title="Sub2",
                        SubNodes=new ObservableCollection<Node>()
                        {
                            new Node()
                            {
                                Title="1"
                            },
                            new Node()
                            {
                                Title="2"
                            },
                            new Node()
                            {
                                Title ="3"
                            }
                        }
                    }
                }
            });
        }

        #region Command
        public void ExitAction()
        {
            
        }

        public void DiscoverAction()
        {

        }

        public void ConnectAction()
        {

        }
        #endregion
    }
}