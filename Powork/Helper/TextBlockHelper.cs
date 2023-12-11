﻿using Powork.Control;
using Powork.Model;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;

namespace Powork.Helper
{
    static public class TextBlockHelper
    {
        static public TextBlock GetTimeControl(UserMessage userMessage)
        {
            TextBlock timeTextBlock = null;

            Application.Current.Dispatcher.Invoke(() =>
            {
                timeTextBlock = new TextBlock();
                if (userMessage.IP == GlobalVariables.SelfInfo[0].IP && userMessage.Name == GlobalVariables.SelfInfo[0].Name)
                {
                    timeTextBlock.HorizontalAlignment = HorizontalAlignment.Right;
                }
                else
                {
                    timeTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
                }
                timeTextBlock.Foreground = Brushes.LightGreen;
                timeTextBlock.Text = userMessage.Time;
            });
            
            return timeTextBlock;
        }
        static public TextBlock GetMessageControl (UserMessage userMessage)
        {
            TextBlock textBlock = null;
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (userMessage.Type == MessageType.Message)
                {
                    List<UserMessageBody> userMessageBodyList = userMessage.MessageBody;
                    textBlock = new SelectableTextBlock();

                    textBlock.Foreground = Brushes.White;

                    foreach (UserMessageBody body in userMessageBodyList)
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
                            Image image = new Image();
                            try
                            {
                                BitmapImage bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                bitmap.UriSource = new Uri(body.Content);
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.EndInit();

                                image.Source = bitmap;

                                // 设置Stretch模式为Uniform，以保持图片的纵横比
                                image.Stretch = Stretch.Uniform;

                                // 检查图片的宽度和高度并相应地设置Image控件的MaxHeight和MaxWidth
                                if (bitmap.PixelWidth > 128 || bitmap.PixelHeight > 128)
                                {
                                    // 计算缩放比例
                                    double scale = Math.Min(128.0 / bitmap.PixelWidth, 128.0 / bitmap.PixelHeight);
                                    image.MaxHeight = bitmap.PixelHeight * scale;
                                    image.MaxWidth = bitmap.PixelWidth * scale;
                                }
                                else
                                {
                                    image.MaxHeight = bitmap.PixelHeight;
                                    image.MaxWidth = bitmap.PixelWidth;
                                }
                            }
                            catch
                            {
                            }
                            image.MouseLeftButtonUp += (s, e) =>
                            {

                            };
                            InlineUIContainer container = new InlineUIContainer(image);
                            textBlock.Inlines.Add(container);
                        }
                        else if (body.Type == ContentType.File)
                        {
                            Image image = new Image();
                            try
                            {
                                BitmapImage bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                bitmap.UriSource = new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image\\file.png"));
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.EndInit();

                                image.Source = bitmap;

                                image.Stretch = Stretch.Uniform;

                                if (bitmap.PixelWidth > 128 || bitmap.PixelHeight > 128)
                                {
                                    double scale = Math.Min(128.0 / bitmap.PixelWidth, 128.0 / bitmap.PixelHeight);
                                    image.MaxHeight = bitmap.PixelHeight * scale;
                                    image.MaxWidth = bitmap.PixelWidth * scale;
                                }
                                else
                                {
                                    image.MaxHeight = bitmap.PixelHeight;
                                    image.MaxWidth = bitmap.PixelWidth;
                                }
                            }
                            catch
                            {
                            }
                            image.Cursor = Cursors.Hand;
                            image.PreviewMouseUp += (s, e) =>
                            {
                                e.Handled = true;
                            };
                            InlineUIContainer container = new InlineUIContainer(image);
                            textBlock.Inlines.Add(container);

                            textBlock.Inlines.Add(new LineBreak());

                            FileAttributes attr = File.GetAttributes(body.Content);
                            string path = body.Content;
                            string name = new DirectoryInfo(path).Name;
                            Run run = new Run(name);
                            textBlock.Inlines.Add(run);
                        }
                    }

                    if (userMessage.IP == GlobalVariables.SelfInfo[0].IP && userMessage.Name == GlobalVariables.SelfInfo[0].Name)
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
                    List<UserMessageBody> userMessageBodyList = userMessage.MessageBody;
                    textBlock = new TextBlock();

                    textBlock.Foreground = Brushes.Pink;

                    foreach (UserMessageBody body in userMessageBodyList)
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
