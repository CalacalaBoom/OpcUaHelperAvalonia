using app.Core.Models;
using Avalonia.Controls.Notifications;
using OpcUaHelper;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;

namespace app.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Binding
        private string? _Address;
        public string? Address
        {
            get => _Address;
            set => this.RaiseAndSetIfChanged(ref _Address, value);
        }

        private bool _isFastAccess = false;
        public bool isFastAccess
        {
            get => _isFastAccess;
            set => this.RaiseAndSetIfChanged(ref _isFastAccess, value);
        }

        private string? _Delay;
        public string? Delay
        {
            get => _Delay;
            set => this.RaiseAndSetIfChanged(ref _Delay, value);
        }

        private ObservableCollection<Node> _Nodes = new ObservableCollection<Node>();
        public ObservableCollection<Node> Nodes
        {
            get => _Nodes;
            set => this.RaiseAndSetIfChanged(ref _Nodes, value);
        }

        private Node _SelectedNode = new Node();
        public Node SelectedNode
        {
            get => _SelectedNode;
            set
            {
                this.RaiseAndSetIfChanged(ref _SelectedNode, value);
                SelectedChange();
            }

        }

        private string? _NodeAddress;
        public string? NodeAddress
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

        private WindowNotificationManager? _manager;
        private OpcUaClient m_OpcUaClient;

        public MainViewModel(WindowNotificationManager? manager)
        {
            _manager = manager;

            Delay = "0";

            m_OpcUaClient = new OpcUaClient();
            m_OpcUaClient.ConnectComplete += M_OpcUaClient_ConnectComplete;
        }

        /// <summary>
        /// 服务器连接成功
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void M_OpcUaClient_ConnectComplete(object? sender, EventArgs e)
        {

        }

        #region Command
        /// <summary>
        /// 菜单退出
        /// </summary>
        public void ExitAction()
        {
        }

        /// <summary>
        /// 发现服务
        /// </summary>
        public void DiscoverAction()
        {

        }

        /// <summary>
        /// 连接动作
        /// </summary>
        public async void ConnectAction()
        {
            try
            {
                await m_OpcUaClient.ConnectServer(Address);
            }
            catch (Exception ex)
            {
                _manager?.Show(new Notification("错误", ex.Message, NotificationType.Error));

            }

        }

        /// <summary>
        /// 选择节点动作
        /// </summary>
        private void SelectedChange()
        {

        }
        #endregion
    }
}