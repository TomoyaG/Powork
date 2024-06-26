﻿using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Ookii.Dialogs.Wpf;
using Powork.Constant;
using Powork.Control;
using Powork.Model;
using Wpf.Ui.Controls;

namespace Powork.Helper
{
    public static class TextBlockHelper
    {
        public static TextBlock GetTimeControl(TCPMessage userMessage, bool showName = false)
        {
            TextBlock timeTextBlock = null;

            Application.Current.Dispatcher.Invoke(() =>
            {
                timeTextBlock = new TextBlock();
                if (userMessage.SenderIP == GlobalVariables.SelfInfo[0].IP && userMessage.SenderName == GlobalVariables.SelfInfo[0].Name)
                {
                    timeTextBlock.HorizontalAlignment = HorizontalAlignment.Right;
                }
                else
                {
                    timeTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
                }
                timeTextBlock.Foreground = Brushes.LightGreen;
                string timeStr = null;
                if (DateTime.TryParse(userMessage.Time, out DateTime dateTime))
                {
                    timeStr = dateTime.ToString(Format.DateTimeFormatWithSeconds);
                }
                if (showName)
                {
                    timeTextBlock.Text = userMessage.SenderName + " [" + timeStr + "]";
                }
                else
                {
                    timeTextBlock.Text = timeStr;
                }
            });

            return timeTextBlock;
        }
        public static TextBlock GetMessageControl(TCPMessage userMessage)
        {
            TextBlock textBlock = null;
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (userMessage.Type == MessageType.UserMessage || userMessage.Type == MessageType.TeamMessage)
                {
                    List<TCPMessageBody> userMessageBodyList = userMessage.MessageBody;
                    textBlock = new SelectableTextBlock();

                    textBlock.Foreground = Brushes.White;

                    foreach (TCPMessageBody body in userMessageBodyList)
                    {
                        if (textBlock.Inlines.Count > 0)
                        {
                            textBlock.Inlines.Add(new LineBreak());
                        }

                        if (body.Type == ContentType.Text)
                        {
                            Run run = new Run(body.Content);
                            textBlock.Inlines.Add(run);
                        }
                        else if (body.Type == ContentType.Picture)
                        {
                            InlineUIContainer container = new InlineUIContainer(ButtonHelper.CreateImageButton(body.Content, new RoutedEventHandler((s, e) =>
                            {
                                if (!System.IO.Path.Exists(body.Content))
                                {
                                    System.Windows.MessageBox.Show("No such file: " + body.Content);
                                    return;
                                }
                                Process p = new Process();
                                p.StartInfo = new ProcessStartInfo(body.Content)
                                {
                                    UseShellExecute = true
                                };
                                p.Start();
                            })));
                            textBlock.Inlines.Add(container);
                        }
                        else if (body.Type == ContentType.File)
                        {
                            InlineUIContainer container = new InlineUIContainer(ButtonHelper.CreateImageButton(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image\\file.png"), new RoutedEventHandler((s, e) =>
                            {
                                var fbd = new VistaFolderBrowserDialog
                                {
                                    SelectedPath = AppDomain.CurrentDomain.BaseDirectory,
                                    Description = "Select a folder",
                                    UseDescriptionForTitle = true
                                };

                                bool result = (bool)fbd.ShowDialog();

                                if (result && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                                {
                                    GlobalVariables.TcpServerClient.RequestFile(body.ID, userMessage.SenderIP, GlobalVariables.TcpPort, fbd.SelectedPath);
                                }
                            })));
                            textBlock.Inlines.Add(container);

                            textBlock.Inlines.Add(new LineBreak());

                            string name = body.Content;
                            Run run = new Run(name);
                            textBlock.Inlines.Add(run);
                        }
                    }

                    if (userMessage.SenderIP == GlobalVariables.SelfInfo[0].IP && userMessage.SenderName == GlobalVariables.SelfInfo[0].Name)
                    {
                        textBlock.HorizontalAlignment = HorizontalAlignment.Right;
                    }
                    else
                    {
                        textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                    }
                }
                else if (userMessage.Type == MessageType.Error)
                {
                    List<TCPMessageBody> userMessageBodyList = userMessage.MessageBody;
                    textBlock = new TextBlock();

                    textBlock.Foreground = Brushes.Pink;

                    foreach (TCPMessageBody body in userMessageBodyList)
                    {
                        if (textBlock.Inlines.Count > 0)
                        {
                            textBlock.Inlines.Add(new LineBreak());
                        }

                        Run run = new Run(body.Content);
                        textBlock.Inlines.Add(run);
                    }

                    textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                }
            });


            return textBlock;
        }
    }
}
