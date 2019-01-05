using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Logging;

namespace FileManagerLibrary.Base.Serialization
{

    public class Serializer<TypeObj>
    {
        public Serializer()
        {
        }

        public void SerializeObject(string filename, TypeObj objectToSerialize)
        {
            Stream stream;
            try
            {
                stream = File.Open(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            }
            catch
            {
                File.Delete(filename);
                stream = File.Open(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            }
            BinaryFormatter bFormatter = new BinaryFormatter();
            bFormatter.Serialize(stream, objectToSerialize);
            stream.Close();
        }

        public TypeObj DeSerializeObject(string filename)
        {
            Stream stream = null;
            try
            {
                TypeObj objectToSerialize;
                if(File.Exists(filename) == false)
                    return (TypeObj)Activator.CreateInstance(typeof(TypeObj));
                stream = File.Open(filename, FileMode.Open);
                BinaryFormatter bFormatter = new BinaryFormatter();
                objectToSerialize = (TypeObj)bFormatter.Deserialize(stream);
                stream.Close();
                return objectToSerialize;
            }
            catch (Exception ex)
            {
                if (Global.IsWindowsUIInstance == true)
                {
                    if (MessageBox.Show("An error occured during reading the applicaiton data. Do you want to reset the data and continue? (" + filename + ")." + ex.Message, "Critcal Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
                    {
                        Logger.StaticLogger.AddErrorLogEntry(ex, "Critical Error during retriving app settings. Cannot continue");
                        Application.Exit();
                    }
                    else
                    {
                        stream.Close();
                        stream = null;
                        string backupfile = filename + " - " + DateTime.Now.ToString().Replace("/", "-").Replace(":", " ") + ".Backup";
                        File.Copy(filename, backupfile);
                        File.Delete(filename);
                        MessageBox.Show("Resetting user data complete. A backup is taken into file '" + backupfile + "'");
                        Logger.StaticLogger.AddErrorLogEntry(ex, "Critical Error during retriving app settings. Resetting user data complete. A backup is taken into file '" + backupfile + "'");
                    }
                }
                else
                {
                    Logger.StaticLogger.AddErrorLogEntry(ex, "Critical Error during retriving app settings. Cannot continue");
                    Application.Exit();
                }
                return (TypeObj)Activator.CreateInstance(typeof(TypeObj));
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }
    }
}
