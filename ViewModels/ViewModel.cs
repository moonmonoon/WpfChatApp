using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfChatApp.Commands;
using WpfChatApp.CustomControls;
using WpfChatApp.Models;

namespace WpfChatApp.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        #region MainWindow

        #region Properties
        public string ContactName { get; set; }
        public Uri ContactPhoto { get; set; }
        public string LastSeen { get; set; }
        #endregion

        #endregion


        #region Status Thumbs

        #region Properties
        public ObservableCollection<StatusDataModel> statusThumbsCollection { get; set; }
        #endregion

        #region Logics
        void LoadStatusThumbs()
        {
            // Lets bind our collection  to itemscontrol
            statusThumbsCollection = new ObservableCollection<StatusDataModel>()
            {
                // Since we want to keep first status blank for the user to add own status
                new StatusDataModel
                {
                    IsMeAddStatus=true,
                },
                new StatusDataModel
                {
                  ContactName="Mike",
                   ContactPhoto=new Uri("/assets/1.png", UriKind.RelativeOrAbsolute),
                     StatusImage=new Uri("/assets/5.jpg", UriKind.RelativeOrAbsolute),
                    IsMeAddStatus=false
                },
                new StatusDataModel
                {
                  ContactName="Steve",
                   ContactPhoto=new Uri("/assets/2.jpg", UriKind.RelativeOrAbsolute),
                     StatusImage=new Uri("/assets/8.jpg", UriKind.RelativeOrAbsolute),
                    IsMeAddStatus=false
                },
                new StatusDataModel
                {
                  ContactName="Will",
                   ContactPhoto=new Uri("/assets/3.png", UriKind.RelativeOrAbsolute),
                     StatusImage=new Uri("/assets/5.jpg", UriKind.RelativeOrAbsolute),
                    IsMeAddStatus=false
                },

                new StatusDataModel
                {
                  ContactName="John",
                   ContactPhoto=new Uri("/assets/4.png", UriKind.RelativeOrAbsolute),
                     StatusImage=new Uri("/assets/3.jpg", UriKind.RelativeOrAbsolute),
                    IsMeAddStatus=false
                },
                };
                OnPropertyChanged("statusThumbsCollection");
        }
        #endregion

        #endregion


        #region Chats List

        #region Properties

        public ObservableCollection<ChatListData> mChats;
        public ObservableCollection<ChatListData> Chats
        {
            get => mChats;
            set
            {
                //To Change the list
                if (mChats == value)
                    return;

                //To Update the list
                mChats = value;

                OnPropertyChanged("Chats");
            }
        }

        #endregion

        #region Logics
        void LoadChats()
        {
            Chats = new ObservableCollection<ChatListData>()
            {
                new ChatListData{
                ContactName = "Billy",
                ContactPhoto = new Uri("/assets/6.jpg", UriKind.RelativeOrAbsolute),
                Message="Hey, What's Up?",
                LastMessageTime="Tue, 12:58 PM",
                ChatIsSelected=true
                },
                new ChatListData{
                ContactName = "Mike",
                ContactPhoto = new Uri("/assets/1.png", UriKind.RelativeOrAbsolute),
                Message="Check the mail.",
                LastMessageTime="Mon, 10:07 AM"
                },
                new ChatListData{
                ContactName = "Steve",
                ContactPhoto = new Uri("/assets/7.png", UriKind.RelativeOrAbsolute),
                Message="Yes, we had fun.",
                LastMessageTime="Tue, 08:10 AM"
                },
                new ChatListData{
                ContactName = "John",
                ContactPhoto = new Uri("/assets/8.jpg", UriKind.RelativeOrAbsolute),
                Message="What about you?",
                LastMessageTime="Tue, 01:00 PM"
                }
            };
            OnPropertyChanged("Chats");
        }
        #endregion

        #region Commands
        // To get the ContactName of selected chat so that we can open corresponding conversation
        protected ICommand _getSelectedChatCommand;

        // ??= 구문은 c# 8 이상에서 사용 가능
        //public ICommand GetSelectedChatCommand => _getSelectedChatCommand ??= new RelayCommand(parameter =>
        //{
        //    if (parameter is ChatListData v)
        //    {
        //        //getting contactname from selected chat
        //        ContactName = v.ContactName;
        //        OnPropertyChanged("ContactName");

        //        //getting contactphoto from selected chat
        //        ContactPhoto = v.ContactPhoto;
        //        OnPropertyChanged("ContactPhoto");
        //    }
        //});
        public ICommand GetSelectedChatCommand
        {
            get
            {
                if (_getSelectedChatCommand == null)
                {
                    _getSelectedChatCommand = new RelayCommand(parameter =>
                    {
                        // getting param from selected chat
                        if (parameter is ChatListData v)
                        {
                            ContactName = v.ContactName;
                            OnPropertyChanged("ContactName");

                            ContactPhoto = v.ContactPhoto;
                            OnPropertyChanged("ContactPhoto");
                        }
                    });
                }
                return _getSelectedChatCommand;
            }
        }
        #endregion

        #endregion


        #region Conversations

        #region Properties
        protected ObservableCollection<ChatConversation> mConversations;
        public ObservableCollection<ChatConversation> Conversations
        {
            get => mConversations;
            set
            {
                mConversations = value;
                OnPropertyChanged("Conversations");
            }
        }
        #endregion

        #region Logics

        void LoadChatConversation()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();

            if (Conversations == null)
                Conversations = new ObservableCollection<ChatConversation>();

            Conversations.Clear();
            using (SqlCommand com = new SqlCommand("select * from conversations where ContactName='Mike'", connection))
            {
                using (SqlDataReader reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string MsgReceivedOn = !string.IsNullOrEmpty(reader["MsgReceivedOn"].ToString()) ?
                            Convert.ToDateTime(reader["MsgReceivedOn"].ToString()).ToString("MMM dd, hh:mm tt") : "";

                        string MsgSentOn = !string.IsNullOrEmpty(reader["MsgSentOn"].ToString()) ?
                            Convert.ToDateTime(reader["MsgSentOn"].ToString()).ToString("MMM dd, hh:mm tt") : "";

                        var conversation = new ChatConversation()
                        {
                            ContactName = reader["ContactName"].ToString(),
                            ReceivedMessage = reader["ReceivedMsgs"].ToString(),
                            MsgReceivedOn = MsgReceivedOn,
                            SentMessage = reader["SentMsgs"].ToString(),
                            MsgSentOn = MsgSentOn,
                            IsMessageReceived = string.IsNullOrEmpty(reader["ReceivedMsgs"].ToString()) ? false : true
                        };

                        Conversations.Add(conversation);
                        OnPropertyChanged("Conversations");
                    }
                }
            }
        }

        #endregion

        #endregion

        // using database containing contact details & conversations
        SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\MoonMoNoon\WPF\WpfChatApp\Database\Database1.mdf;Integrated Security=True");

        public ViewModel()
        {
            LoadStatusThumbs();
            LoadChats();
            LoadChatConversation();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
