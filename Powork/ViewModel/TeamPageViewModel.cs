﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Powork.Helper;
using Powork.Model;
using Powork.Repository;
using Powork.ViewModel.Inner;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;

namespace Powork.ViewModel
{
    class TeamPageViewModel : ObservableObject
    {
        private int firstMessageID = -1;
        private TeamViewModel nowTeam = new TeamViewModel() { ID = "1", Name = "1" };
        public ObservableCollection<TextBlock> messageList;
        public ObservableCollection<TextBlock> MessageList
        {
            get
            {
                return messageList;
            }
            set
            {
                SetProperty<ObservableCollection<TextBlock>>(ref messageList, value);
            }
        }

        private ObservableCollection<TeamViewModel> teamList;
        public ObservableCollection<TeamViewModel> TeamList
        {
            get
            {
                return teamList;
            }
            set
            {
                SetProperty<ObservableCollection<TeamViewModel>>(ref teamList, value);
            }
        }
        private bool pageEnabled;
        public bool PageEnabled
        {
            get
            {
                return pageEnabled;
            }
            set
            {
                SetProperty<bool>(ref pageEnabled, value);
            }
        }
        private bool sendEnabled;
        public bool SendEnabled
        {
            get
            {
                return sendEnabled;
            }
            set
            {
                SetProperty<bool>(ref sendEnabled, value);
            }
        }
        private FlowDocument richTextBoxDocument;
        public FlowDocument RichTextBoxDocument
        {
            get { return richTextBoxDocument; }
            set
            {
                SetProperty<FlowDocument>(ref richTextBoxDocument, value);
            }
        }
        public ICommand WindowLoadedCommand { get; set; }
        public ICommand WindowClosingCommand { get; set; }
        public ICommand WindowClosedCommand { get; set; }
        public ICommand TeamClickCommand { get; set; }
        public ICommand ScrollAtTopCommand { get; set; }

        public TeamPageViewModel()
        {
            PageEnabled = true;
            SendEnabled = false;
            RichTextBoxDocument = new FlowDocument();

            MessageList = new ObservableCollection<TextBlock>();

            WindowLoadedCommand = new RelayCommand<RoutedEventArgs>(WindowLoaded);
            WindowClosingCommand = new RelayCommand<CancelEventArgs>(WindowClosing);
            WindowClosedCommand = new RelayCommand(WindowClosed);
            TeamClickCommand = new RelayCommand<TeamViewModel>(TeamClick);
            ScrollAtTopCommand = new RelayCommand(ScrollAtTop);

            TeamList = new ObservableCollection<TeamViewModel>();
        }

        private void WindowLoaded(RoutedEventArgs eventArgs)
        {
            List<Team> teamList = new List<Team>();
            foreach (Team team in teamList)
            {
                TeamList.Add(new TeamViewModel(team));
            }
        }

        private void WindowClosing(CancelEventArgs eventArgs)
        {
        }

        private void WindowClosed()
        {
        }

        private void TeamClick(TeamViewModel teamViewModel)
        {
            MessageList.Clear();
            nowTeam.Selected = false;
            nowTeam = teamViewModel;
            nowTeam.Selected = true;
            SendEnabled = true;

            List<TCPMessage> messageList = TeamMessageRepository.SelectMessgae(nowTeam.ID);
            if (messageList != null && messageList.Count >= 1)
            {
                firstMessageID = messageList[0].ID;
            }

            foreach (TCPMessage message in messageList)
            {
                if (message.Type == MessageType.UserMessage)
                {
                    TextBlock timeTextBlock = TextBlockHelper.GetTimeControl(message);
                    TextBlock textBlock = TextBlockHelper.GetMessageControl(message);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageList.Add(timeTextBlock);
                        MessageList.Add(textBlock);
                    });
                }
                else if (message.Type == MessageType.Error)
                {
                    TextBlock textBlock = TextBlockHelper.GetMessageControl(message);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageList.Add(textBlock);
                    });
                }
            }
        }







        private void ScrollAtTop()
        {
            if (firstMessageID == -1)
            {
                return;
            }

            List<TCPMessage> messageList = TeamMessageRepository.SelectMessgae(nowTeam.ID, firstMessageID);
            if (messageList != null && messageList.Count >= 1)
            {
                firstMessageID = messageList[0].ID;
            }
            int index = 0;
            foreach (TCPMessage message in messageList)
            {
                if (message.Type == MessageType.UserMessage)
                {
                    TextBlock timeTextBlock = TextBlockHelper.GetTimeControl(message);
                    TextBlock textBlock = TextBlockHelper.GetMessageControl(message);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageList.Insert(index++, timeTextBlock);
                        MessageList.Insert(index++, textBlock);
                    });
                }
                else if (message.Type == MessageType.Error)
                {
                    TextBlock textBlock = TextBlockHelper.GetMessageControl(message);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageList.Insert(index++, textBlock);
                    });
                }
            }
        }
    }
}
