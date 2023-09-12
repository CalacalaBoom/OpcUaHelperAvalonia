using app.Converter;
using app.Core.Models;
using app.Core.Services;
using Avalonia.Controls.Notifications;
using Avalonia.Media.Imaging;
using DynamicData;
using Opc.Ua;
using OpcUaHelper;
using Org.BouncyCastle.Asn1.X509;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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

        private bool _isSucess = false;
        public bool isSucess
        {
            get => _isSucess;
            set => this.RaiseAndSetIfChanged(ref _isSucess, value);
        }

        private string? _Delay;
        public string? Delay
        {
            get => _Delay;
            set => this.RaiseAndSetIfChanged(ref _Delay, value);
        }

        private ObservableCollection<Core.Models.Node> _Nodes = new ObservableCollection<Core.Models.Node>();
        public ObservableCollection<Core.Models.Node> Nodes
        {
            get => _Nodes;
            set => this.RaiseAndSetIfChanged(ref _Nodes, value);
        }

        private Core.Models.Node _SelectedNode = new Core.Models.Node();
        public Core.Models.Node SelectedNode
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
            _manager?.Show(new Notification("Success", "Connected", NotificationType.Success));
            Tip = $"OPC Status: [正常] {DateTime.Now} Connected [{Address}]";
            isSucess = true;

            LoadNodes(Nodes, ObjectIds.ObjectsFolder);
        }


        private int root = 0;

        /// <summary>
        /// 加载节点
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="sourceId"></param>
        private void LoadNodes(ObservableCollection<Core.Models.Node> nodes, NodeId sourceId)
        {
            ReferenceDescriptionCollection references = OpcUaServices.GetReferenceDescriptionCollection(m_OpcUaClient, sourceId);
            if (references != null)
            {
                if (root == 0)
                {
                    for (int ii = 0; ii < references.Count; ii++)
                    {
                        //节点添加
                        var targe = references[ii];
                        Core.Models.Node node = new Core.Models.Node();
                        node.Title = targe.DisplayName.Text;
                        node.NodeId = (NodeId)targe.NodeId;
                        node.Image = ImageHelper.LoadFromResource(new Uri("resm:app.Core.images.Root.png?assembly=app.Core"));
                        node.SubNodes = new ObservableCollection<Core.Models.Node>() { new Core.Models.Node() };
                        nodes.Add(node);
                    }
                    root++;
                    return;
                }
                for (int ii = 0; ii < references.Count; ii++)
                {
                    //节点添加
                    var targe = references[ii];
                    Core.Models.Node node = new Core.Models.Node();
                    node.Title = targe.DisplayName.Text;
                    node.NodeId = (NodeId)targe.NodeId;
                    if (targe.NodeClass == NodeClass.Variable)
                        node.Image = ImageHelper.LoadFromResource(new Uri("resm:app.Core.images.Point.png?assembly=app.Core"));
                    else
                    {
                        node.Image = ImageHelper.LoadFromResource(new Uri("resm:app.Core.images.Node.png?assembly=app.Core"));
                        node.SubNodes = new ObservableCollection<Core.Models.Node>() { new Core.Models.Node() };
                    }
                    nodes.Add(node);
                }
            }
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
                if (m_OpcUaClient.Connected) return;
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
            NodeAddress = SelectedNode.NodeId.ToString();
            SelectedNode.SubNodes.Clear();
            LoadNodes(SelectedNode.SubNodes, NodeAddress);
            LoadDataGrid();
        }

        private async void LoadDataGrid()
        {
            VariableList.Clear();
            int index = 0;
            ReferenceDescriptionCollection references = OpcUaServices.GetReferenceDescriptionCollection(m_OpcUaClient, NodeAddress);

            if (references?.Count > 0)
            {
                // 获取所有要读取的子节点
                List<NodeId> nodeIds = new List<NodeId>();
                for (int ii = 0; ii < references.Count; ii++)
                {
                    ReferenceDescription target = references[ii];
                    nodeIds.Add((NodeId)target.NodeId);
                }

                DateTime dateTimeStart = DateTime.Now;

                // 获取所有的值
                DataValue[] dataValues = await Task.Run(() =>
                {
                    return OpcUaServices.ReadOneNodeFiveAttributes(m_OpcUaClient, nodeIds);
                });

                Delay = ((int)(DateTime.Now - dateTimeStart).TotalMilliseconds).ToString();// 显示

                for (int jj = 0; jj < dataValues.Length; jj += 5)
                {
                    var data = BuildDataGrid(dataValues, jj, index++, nodeIds[jj / 5]);
                    VariableList.Add(data);
                }
            }
        }

        private string isEmpty(Object obj) => obj == null ? "" : obj.ToString();

        private VariableData BuildDataGrid(DataValue[] dataValues, int startIndex, int rowid, NodeId nodeId)
        {
            if (dataValues[startIndex].WrappedValue.Value == null) return null;
            NodeClass nodeclass = (NodeClass)dataValues[startIndex].WrappedValue.Value;

            VariableData data = new VariableData();
            //public string Name { get; set; }
            //public string Value { get; set; }
            //public string Type { get; set; }
            //public string AccessLevelt { get; set; }
            //public string Description { get; set; }
            data.Name = isEmpty(dataValues[3 + startIndex].WrappedValue.Value);
            data.Description = isEmpty(dataValues[4 + startIndex].WrappedValue.Value);
            data.AccessLevelt = OpcUaServices.GetDiscriptionFromAccessLevel(dataValues[2 + startIndex]);

            if (nodeclass == NodeClass.Object || nodeclass == NodeClass.Method)
            {
                data.Value = "";
                data.Type = isEmpty(nodeclass);
            }
            else if (nodeclass == NodeClass.Variable)
            {
                DataValue dataValue = dataValues[1 + startIndex];

                if (dataValue.WrappedValue.TypeInfo != null)
                {
                    data.Type = isEmpty(dataValue.WrappedValue.TypeInfo.BuiltInType);
                    if (dataValue.WrappedValue.TypeInfo.ValueRank == ValueRanks.Scalar)
                    {
                        data.Value = isEmpty(dataValue.WrappedValue.Value);
                    }
                    else if (dataValue.WrappedValue.TypeInfo.ValueRank == ValueRanks.OneDimension)
                    {
                        data.Value = isEmpty(dataValue.Value.GetType());
                    }
                    else if (dataValue.WrappedValue.TypeInfo.ValueRank == ValueRanks.TwoDimensions)
                    {
                        data.Value = isEmpty(dataValue.Value.GetType());
                    }
                    else
                    {
                        data.Value = isEmpty(dataValue.Value.GetType());
                    }
                }
                else
                {
                    data.Value = isEmpty(dataValue.Value);
                    data.Type = "null";
                }
            }
            else
            {
                data.Value = "";
                data.Type = isEmpty(nodeclass);
            }

            return data;
        }
        #endregion
    }
}