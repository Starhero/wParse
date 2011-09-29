using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using WParse;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Threading;

namespace EWOS
{
    public partial class FormMain
    {
        private void LoadAndParse()
        {
            //SplashScreen.ShowSplashScreen();
            

            
            if (Directory.Exists(@"parsers\"))
            {
                if (Directory.GetFiles(@"Parsers\", "*.cs").Length != 0 || Directory.GetFiles(@"parsers\", "*.wsp").Length != 0)
                {
                    //TODO: Need to find a way to get WSP files and CS files. Simple way would be and array added to the IEnumerable.
                    SplashScreen.SplashScreen.SetStatus("Loading Parsers....");
                    IEnumerable<string> parsers = Directory.GetFiles(@"Parsers\", "*.cs");

                    if (Directory.Exists(FullPath + @"\logs\"))
                    {

                        SplashScreen.SplashScreen.SetStatus("Loading Logs");
                        if (Directory.GetFiles(FullPath + "\\logs\\", "_Event*.txt").Length != 0 || Directory.GetFiles(FullPath + "\\logs\\", "_Skills*.txt").Length !=0)
                        {
                            IEnumerable<string> wEvent = Directory.GetFiles(FullPath + "\\logs\\", "_Event*.txt");
                            IEnumerable<string> wskill = Directory.GetFiles(FullPath + "\\logs\\", "_Skills*.txt");
                            AggregateParser parser = null;
                            
                            try
                            {
                                parser = new AggregateParser(wEvent,wskill, parsers);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(
                                    "EXCEPTION THROWN: We didn't complie one of the scripts correctly or it has errors." +
                                    ex.Message);


                            }
                            SplashScreen.SplashScreen.SetStatus("Parsing....");
                                parser.Parse();



                                SplashScreen.SplashScreen.SetStatus("Sorting and treeing");
                                Dictionary<string, Queue<DateTime>> entries = parser.Proxy.GetEntries();
                                foreach (KeyValuePair<string, List<string>> pair in 
                                    parser.Proxy.GetLinks())
                                {
                                    TreeNode Parent = new TreeNode(pair.Key);

                                    foreach (string key in pair.Value)
                                    {
                                        TreeNode child = new TreeNode(key);

                                        long count;
                                        if (!parser.Proxy.TryGetCount(key, out count))
                                        {
                                            continue;
                                        }

                                        Parent.Nodes.Add(String.Format("{0}:{1}", child.Text, count));

                                    }
                                    treeView1.Nodes.Add(Parent);
                                    treeView1.Sort();

                            }
                                SplashScreen.SplashScreen.SetStatus("FINISHED :D");
                                SplashScreen.SplashScreen.CloseForm();

                        }
                    }

                    else
                    {
                        MessageBox.Show(this, @"ERROR: Seems you haven't ever logged on to this User, so there is nothing to parse, unless " + FullPath + @"\logs\" +" directory doesn't exsist or is moved." , "What the hell!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }

            }
            else
            {
                MessageBox.Show(this, "ERROR: I got no parsers to use! I can't run!/r/n/r/nPlease reinstall the application!", "SHEEEET", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private void initialload()
        {
            ////////////////////////////////////////////////////
            //Old Way is commented ---------------------------- changed 9/27/11 (elenin SHOULD be eclipsing us..but so far naaa)
            ////////////////////////////////////////////////////
            //string dir = Environment.GetEnvironmentVariable("USERPROFILE");

            //Start Reg object
            RegistryKey regkey;

            //Load wurms current directory TODO: Add an execption for if this doesn't work
            regkey = Registry.CurrentUser.OpenSubKey(@"Software\JavaSoft\Prefs\com\wurmonline\client");

            // RegDir should now have the installion directory. Direct.
            string RegDir = regkey.GetValue("wurm_dir").ToString();
            //Got the last ran user too!
            string RegUser = regkey.GetValue("wurm_user").ToString();
            //Knocking out a rolfifide charactor, stupid java
            RegDir = RegDir.Remove(0, 1);
            //and again.
            RegUser = RegUser.Remove(0, 1);

            //More the same shit ..Cant have paths = C:///user/// ..etc

            string pattern = @"///?";

            RegDir = Regex.Replace(RegDir, pattern, @"\");

            //Now make the main string this.
            this.Path = RegDir;
            this.User = RegUser;
            this.FullPath = RegDir + "\\players\\" + User;
            //this.LoadAndParse();

                if (Directory.Exists(this.Path + @"\players"))
                {
                    string[] accountsdir = Directory.GetDirectories(RegDir + @"\players\");
                    int lnth = accountsdir.GetLength(0);
                    string[] accounts = new string[lnth];

                    for (int i = 0; i < accountsdir.Length; i++)
                    {
                        int start = accountsdir[i].LastIndexOf("\\");
                        accounts[i] = accountsdir[i].Substring(start + 1);
                    }

                    //for (int i = 0; i < accounts.Length; i++)
                    //{
                        //if (accounts[i] == RegUser)
                        //{
                            //usr = i;
                        //}
                    //}
                    this.cbAccounts.Items.AddRange(accounts);
                    this.cbAccounts.SelectedItem = RegUser;
                    //this.Path = RegDir + "\\players\\" + RegUser + "\\";
                    
                    
                    //MessageBox.Show("Wurm folder found!\r\n\r\nWelcome to Wurm Statistics!\r\n\r\n Since this is the first time you have ran the program, we need to know what logs you want to read from.\r\n\r\n In the next dialog, please specify your Wurm charactors event log folder.");

                    //FrmSettings initset = new FrmSettings(this.Path);
                    //initset.StartPosition = FormStartPosition.CenterParent;
                    //initset.ShowDialog();

                    
                    // ReSharper disable RedundantThisQualifier
                    
                }
                else
                {
                    this.Path = @"C:\";
                    MessageBox.Show("Wurm folder not found!\r\n\r\nWelcome to Wurm Statistics!\r\n\r\n Since this is the first time you have ran the program, we need to know what logs you want to read from.\r\n\r\n In the next dialog, please specify your Wurm charactors event log folder.");

                    FrmSettings initset = new FrmSettings(this.Path);
                    initset.ShowDialog();
                }
            }
        }
    }