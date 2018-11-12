using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media;
using System.Xml;
using Microsoft.Win32;

namespace Task_Layout_Manager
{
    internal class FileIo
    {
        public static void SaveXml(List<TaskWindow> tasks)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "TLM file (*.TLM)|*.TLM",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                Title = "Load TLM file"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "\t",
                    NewLineOnAttributes = false
                };

                using (XmlWriter writer = XmlWriter.Create(saveFileDialog.FileName, xmlWriterSettings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("TASK_LAYOUT_MANAGER");

                    foreach (TaskWindow tw in tasks)
                    {
                        writer.WriteStartElement("WINDOW");
                        writer.WriteAttributeString("NAME", tw.Name);
                        writer.WriteAttributeString("COMMANDLINE", tw.Path);
                        writer.WriteStartElement("POSITION");
                        writer.WriteAttributeString("X", tw.PosX.ToString());
                        writer.WriteAttributeString("Y", tw.PosY.ToString());
                        writer.WriteAttributeString("WIDTH", tw.Width.ToString());
                        writer.WriteAttributeString("HEIGHT", tw.Height.ToString());
                        writer.WriteAttributeString("STATE", tw.ShowCmd.ToString());
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
        }

        public static List<TaskWindow> ReadXml(string path)
        {
            List<TaskWindow> taskWindows = new List<TaskWindow>();

            string commandline = "";
            int x = 0, y = 0, width = 0, height = 0, state = 0;
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                if (node.Name == "WINDOW")
                {
                    var name = node.Attributes["NAME"]?.InnerText;
                    commandline = node.Attributes["COMMANDLINE"]?.InnerText;
                    foreach (XmlNode childnode in node)
                    {
                        x = int.Parse(childnode.Attributes["X"]?.InnerText);
                        y = int.Parse(childnode.Attributes["Y"]?.InnerText);
                        width = int.Parse(childnode.Attributes["WIDTH"]?.InnerText);
                        height = int.Parse(childnode.Attributes["HEIGHT"]?.InnerText);
                        state = int.Parse(childnode.Attributes["STATE"]?.InnerText);
                    }

                    ImageSource imgsrc = null;

                    try
                    {
                        imgsrc = ImageConverter.IconToImagesource(Icon.ExtractAssociatedIcon(commandline));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    taskWindows.Add(new TaskWindow(true, name, commandline, 0, state, x, y, height, width, imgsrc));
                }
            }
            return taskWindows;
        }
    }
}