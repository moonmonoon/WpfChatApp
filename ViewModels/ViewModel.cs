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

        #region Search Chats
        protected string LastSearchText { get; set; }
        protected string mSearchText { get; set; }
        public string SearchText
        {
            get => mSearchText;
            set
            {
                //checked if value is different
                if (mSearchText == value)
                    return;

                //update value
                mSearchText = value;

                //if search text is empty restore messages
                if (string.IsNullOrEmpty(SearchText))
                    Search();
            }
        }
        #endregion
        #endregion

        #region Logics
        public void Search()
        {
            //to avoid re-searching same text again
            if ((string.IsNullOrEmpty(LastSearchText) && string.IsNullOrEmpty(SearchText)) || string.Equals(LastSearchText, SearchText))
                return;

            if (string.IsNullOrEmpty(SearchText) || Chats == null || Chats.Count <= 0)
            {
                FilteredChats = new ObservableCollection<ChatListData>(Chats ?? Enumerable.Empty<ChatListData>());
                OnPropertyChanged("FilteredChats");

                FilteredPinnedChats = new ObservableCollection<ChatListData>(PinnedChats ?? Enumerable.Empty<ChatListData>());
                OnPropertyChanged("FilteredPinnedChats");

                //update last search text
                LastSearchText = SearchText;

                return;
            }

            //to find all chats that contain the text in our search box

            // if that chat is in Normal Unpinned Chat list find there...
            FilteredChats = new ObservableCollection<ChatListData>(
                Chats.Where(
                    chat => chat.ContactName.ToLower().Contains(SearchText) // if ContactName contains SearchText then add it in filtered chat list
                            || chat.Message != null && chat.Message.ToLower().Contains(SearchText) // if Message contains SearchText then add it in filtered chat list
                            )
                );
            OnPropertyChanged("FilteredChats");

            // else if not found in Normal Unpinned Chat list, find in pinned chats list
            FilteredPinnedChats = new ObservableCollection<ChatListData>(
                PinnedChats.Where(
                    pinnedChat => pinnedChat.ContactName.ToLower().Contains(SearchText) // if ContactName contains SearchText then add it in filtered chat list
                            || pinnedChat.Message != null && pinnedChat.Message.ToLower().Contains(SearchText) // if Message contains SearchText then add it in filtered chat list
                            )
                );
            OnPropertyChanged("FilteredPinnedChats");

            //update last search text
            LastSearchText = SearchText;
        }
        #endregion

        #region Commands
        protected ICommand _searchCommand;
        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                    _searchCommand = new CommandViewModel(Search);
                return _searchCommand;
            }
            set
            {
                _searchCommand = value;
            }
        }
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

                //Updating filtered chats to match
                FilteredChats = new ObservableCollection<ChatListData>(mChats);
                OnPropertyChanged("Chats");
                OnPropertyChanged("FilteredChats");
            }
        }

        public ObservableCollection<ChatListData> mPinnedChats;
        public ObservableCollection<ChatListData> PinnedChats
        {
            get => mPinnedChats;
            set
            {
                //To Change the list
                if (mPinnedChats == value)
                    return;

                //To Update the list
                mPinnedChats = value;

                //Updating filtered pinned chats to match
                FilteredPinnedChats = new ObservableCollection<ChatListData>(mPinnedChats);
                OnPropertyChanged("PinnedChats");
                OnPropertyChanged("FilteredPinnedChats");
            }
        }

        protected ObservableCollection<ChatListData> _archivedChats;
        public ObservableCollection<ChatListData> ArchivedChats
        {
            get => _archivedChats;
            set
            {
                _archivedChats = value;
                OnPropertyChanged();
            }
        }

        //Filtering Chats & Pinned Chats
        public ObservableCollection<ChatListData> FilteredChats { get; set; }
        public ObservableCollection<ChatListData> FilteredPinnedChats { get; set; }

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
        /// <summary>
        /// To get the ContactName of selected chat so that we can open corresponding conversation
        /// </summary>
        protected ICommand _getSelectedChatCommand;
        // ??= 구문은 c# 8 이상에서 사용 가능
        public ICommand GetSelectedChatCommand => _getSelectedChatCommand ??= new RelayCommand(parameter =>
        {
            if (parameter is ChatListData v)
            {
                //getting contactname from selected chat
                ContactName = v.ContactName;
                OnPropertyChanged("ContactName");

                //getting contactphoto from selected chat
                ContactPhoto = v.ContactPhoto;
                OnPropertyChanged("ContactPhoto");

                LoadChatConversation(v);
            }
        });
        #region - c# 7.3 버전 코드
        //public ICommand GetSelectedChatCommand
        //{
        //    get
        //    {
        //        if (_getSelectedChatCommand == null)
        //        {
        //            _getSelectedChatCommand = new RelayCommand(parameter =>
        //            {
        //                // getting param from selected chat
        //                if (parameter is ChatListData v)
        //                {
        //                    ContactName = v.ContactName;
        //                    OnPropertyChanged("ContactName");

        //                    ContactPhoto = v.ContactPhoto;
        //                    OnPropertyChanged("ContactPhoto");
        //                }
        //            });
        //        }
        //        return _getSelectedChatCommand;
        //    }
        //}
        #endregion

        /// <summary>
        /// To Pin Chat on Pin Button Click
        /// </summary>
        protected ICommand _pinChatCommand;
        public ICommand PinChatCommand => _pinChatCommand ??= new RelayCommand(parameter =>
        {
            if (parameter is ChatListData v)
            {
                if (!FilteredPinnedChats.Contains(v))
                {
                    //add selected chat to pin chat
                    PinnedChats.Add(v);
                    FilteredPinnedChats.Add(v);
                    OnPropertyChanged("PinnedChats");
                    OnPropertyChanged("FilteredPinnedChats");
                    v.ChatIsPinned = true;

                    //remove selected chat from all chats, unpinned chats
                    Chats.Remove(v);
                    FilteredChats.Remove(v);
                    OnPropertyChanged("Chats");
                    OnPropertyChanged("FilteredChats");


                    // Chat will be removed from Pinned List when Archived.. and Vice Versa...
                    // Fixed
                    if (ArchivedChats != null)
                    {
                        if (ArchivedChats.Contains(v))
                        {
                            ArchivedChats.Remove(v);
                            v.ChatIsArchived = false;
                        }
                    }
                }
            }
        });
        protected ICommand _unPinChatCommand;
        public ICommand UnPinChatCommand => _unPinChatCommand ??= new RelayCommand(parameter =>
        {
            if (parameter is ChatListData v)
            {
                if (!FilteredChats.Contains(v))
                {
                    //add selected chat to Normal Unpinned chat list
                    Chats.Add(v);
                    FilteredChats.Add(v);
                    OnPropertyChanged("Chats");
                    OnPropertyChanged("FilteredChats");

                    //remove selected pinned chat list
                    PinnedChats.Remove(v);
                    FilteredPinnedChats.Remove(v);
                    OnPropertyChanged("PinnedChats");
                    OnPropertyChanged("FilteredPinnedChats");
                    v.ChatIsPinned = false;
                }
            }
        });

        /// <summary>
        /// Archive Chat Command
        /// </summary>
        protected ICommand _archiveChatCommand;
        public ICommand ArchiveChatCommand => _archiveChatCommand ??= new RelayCommand(parameter =>
        {
            if (parameter is ChatListData v)
            {
                if (!ArchivedChats.Contains(v))
                {
                    // Chat will be removed from Pinned List when Archived.. and Vice Versa...

                    //add chat in archive list
                    ArchivedChats.Add(v);
                    v.ChatIsArchived = true;
                    v.ChatIsPinned = false;

                    //remove chat from pinned & unpinned chat list
                    Chats.Remove(v);
                    FilteredChats.Remove(v);
                    PinnedChats.Remove(v);
                    FilteredPinnedChats.Remove(v);

                    // update lists
                    OnPropertyChanged("Chats");
                    OnPropertyChanged("FilteredChats");
                    OnPropertyChanged("PinnedChats");
                    OnPropertyChanged("FilteredPinnedChats");
                    OnPropertyChanged("ArchivedChats");
                }
            }
        });
        protected ICommand _unArchiveChatCommand;
        public ICommand UnArchiveChatCommand => _unArchiveChatCommand ??= new RelayCommand(parameter =>
        {
            if (parameter is ChatListData v)
            {
                if (!FilteredChats.Contains(v) && !Chats.Contains(v))
                {
                    Chats.Add(v);
                    FilteredChats.Add(v);
                }

                ArchivedChats.Remove(v);
                v.ChatIsArchived = false;
                v.ChatIsPinned = false;

                OnPropertyChanged("Chats");
                OnPropertyChanged("FilteredChats");
                OnPropertyChanged("ArchivedChats");
            }
        });
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
                //To Change the list
                if (mConversations == value)
                    return;

                //To Update the list
                mConversations = value;

                //Updating filtered pinned chats to match
                FilteredConversations = new ObservableCollection<ChatConversation>(mConversations);
                OnPropertyChanged("Conversations");
                OnPropertyChanged("FilteredConversations");
            }
        }
        public ObservableCollection<ChatConversation> FilteredConversations { get; set; }

        protected string messageText;
        public string MessageText
        {
            get => messageText;
            set
            {
                messageText = value;
                OnPropertyChanged("MessageText");
            }
        }

        protected string LastSearchConversationText;
        protected string mSearchConversationText;
        public string SearchConversationText
        {
            get => mSearchConversationText;
            set
            {
                //checked if value is different
                if (mSearchConversationText == value)
                    return;

                //update value
                mSearchConversationText = value;

                //if search text is empty restore messages
                if (string.IsNullOrEmpty(SearchConversationText))
                    SearchInConversation();
            }
        }
        #endregion

        #region Logics

        void LoadChatConversation(ChatListData chat)
        {
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();

            if (Conversations == null)
                Conversations = new ObservableCollection<ChatConversation>();

            Conversations.Clear();
            FilteredConversations.Clear();
            using (SqlCommand com = new SqlCommand("select * from conversations where ContactName=@ContactName", connection))
            {
                com.Parameters.AddWithValue("@ContactName", chat.ContactName);
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
                        FilteredConversations.Add(conversation);
                        OnPropertyChanged("FilteredConversations");
                    }
                }
            }
        }

        void SearchInConversation()
        {
            //to avoid re-searching same text again
            if ((string.IsNullOrEmpty(LastSearchConversationText) && string.IsNullOrEmpty(SearchConversationText)) || string.Equals(LastSearchConversationText, SearchConversationText))
                return;

            if (string.IsNullOrEmpty(SearchConversationText) || Conversations == null || Conversations.Count <= 0)
            {
                FilteredConversations = new ObservableCollection<ChatConversation>(Conversations ?? Enumerable.Empty<ChatConversation>());
                OnPropertyChanged("FilteredConversations");

                //update last search text
                LastSearchConversationText = SearchConversationText;

                return;
            }

            //to find all Conversations that contain the text in our search box
            FilteredConversations = new ObservableCollection<ChatConversation>(
                Conversations.Where(
                    chat => chat.ReceivedMessage.ToLower().Contains(SearchConversationText) // if ReceivedMessage contains SearchConversationText then add it in filtered chat list
                            || 
                            chat.SentMessage.ToLower().Contains(SearchConversationText) // if SentMessage contains SearchConversationText then add it in filtered chat list
                            )
                );
            OnPropertyChanged("FilteredConversations");

            //update last search text
            LastSearchConversationText = SearchConversationText;
        }
        #endregion

        #region Commands
        protected ICommand _searchConversationCommand;
        public ICommand SearchConversationCommand
        {
            get
            {
                if (_searchConversationCommand == null)
                    _searchConversationCommand = new CommandViewModel(SearchInConversation);
                return _searchConversationCommand;
            }
            set
            {
                _searchConversationCommand = value;
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

            PinnedChats = new ObservableCollection<ChatListData>();
            ArchivedChats = new ObservableCollection<ChatListData>();

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
