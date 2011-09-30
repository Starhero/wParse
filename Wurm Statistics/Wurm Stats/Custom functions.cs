using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using WParse;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Threading;
using Hero;

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
                    Hero.SplashScreen.SetStatus("Loading Parsers....");
                    IEnumerable<string> parsers = Directory.GetFiles(@"Parsers\", "*.cs");

                    if (Directory.Exists(FullPath + @"\logs\"))
                    {

                        Hero.SplashScreen.SetStatus("Loading Logs");
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
                            Hero.SplashScreen.SetStatus("Parsing....");
                                parser.Parse();



                                Hero.SplashScreen.SetStatus("Sorting and treeing");
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
                                Hero.SplashScreen.SetStatus("FINISHED :D");
                                Hero.SplashScreen.CloseForm();

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
            string RegDir;
            string RegUser;
            unrolfify(out RegDir, out RegUser);
            //this.LoadAndParse();
                //Well if the folder doesn't exsist then yeah...nothing we can really do at the momment.
                if (Directory.Exists(this.Path + @"\players"))
                {
                    //Ok so load all the directory paths in \players\ into an arrary of our own
                    string[] accountsdir = Directory.GetDirectories(RegDir + @"\players\");
                    

                    //Make another array from the size of the last one. I could have used MORE regex but..
                    string[] accounts = new string[accountsdir.Length];

                    //Now for the most commonly repeated for statement.
                    for (int i = 0; i < accountsdir.Length; i++)
                    {
                        //Where is the last \\ in the strings?
                        int start = accountsdir[i].LastIndexOf("\\");

                        //Ok cut everything after it, out and slap it into the accounts array equal to the same posistion.
                        accounts[i] = accountsdir[i].Substring(start + 1);
                        //Repeat
                    }

                    //Merge accounts into the combo box named Accounts's list like hot hot molten metal. "Metal must be glowing to combind"
                    this.cbAccounts.Items.AddRange(accounts);

                    //NOTE: This is interesting, seems I can actually refer to an Item in the CB as a string...Well then...text search FTW ..Select the user that was last logged on.
                    this.cbAccounts.SelectedItem = RegUser;

                }
                else
                {

                    //NOTE NOTE NOTE THIS NEEDS SERIOUS WORK. The settings window is being refactored out soon. Redesigned later when I actually have shit to put in it!
                    this.Path = @"C:\";
                    MessageBox.Show("Wurm folder not found!\r\n\r\nWelcome to Wurm Statistics!\r\n\r\n Since this is the first time you have ran the program, we need to know what logs you want to read from.\r\n\r\n In the next dialog, please specify your Wurm charactors event log folder.");

                    FrmSettings initset = new FrmSettings(this.Path);
                    initset.ShowDialog();
                }
            }

        private void unrolfify(out string RegDir, out string RegUser)
        {

            ////////////////////////////////////////////////////
            //Old Way is commented ---------------------------- changed 9/27/11 (elenin SHOULD be eclipsing us..but so far naaa) EDIT 9/26 Still alive..and the world intact...guess that shit was bogus
            ////////////////////////////////////////////////////
            //TODO: Methodfy this mess!
            ////////////////////////////////////////////////////

            //string dir = Environment.GetEnvironmentVariable("USERPROFILE");

            //Start Reg object
            RegistryKey regkey;

            //Load wurms current directory TODO: Add an execption for if this doesn't work
            regkey = Registry.CurrentUser.OpenSubKey(@"Software\JavaSoft\Prefs\com\wurmonline\client");

            // RegDir should now have the installion directory. Direct.
            RegDir = regkey.GetValue("wurm_dir").ToString();
            //Got the last ran user too!
            RegUser = regkey.GetValue("wurm_user").ToString();

            //Knocking out a rolfifide charactor, stupid java
            RegDir = RegDir.Remove(0, 1);

            //checking if there is a / in front of the user name as it seems to differ per user at times??? Does caps matter???
            if (RegUser[0] == '/')
            {
                //Knocking shit out again.
                RegUser = RegUser.Remove(0, 1);
            }
            //More the same shit ..Cant have paths = C:///user/// ..etc

            //So make a regex search pattern here, could have passed it as a direct vairable but eh...I like your memories!
            string pattern = @"///?";

            //MONO WARNING:
            //THIS IS ONLY FOR WINDOWS
            //Replae rolfs shit with real shit.
            RegDir = Regex.Replace(RegDir, pattern, @"\");

            //Now make the main string the reworked string.
            this.Path = RegDir;
            this.User = RegUser;
            this.FullPath = RegDir + "\\players\\" + User;
        }
        }
    }