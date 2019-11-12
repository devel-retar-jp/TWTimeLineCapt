////////////////////////////////////////////////////////////////////////////////////////////////
/// <summary>
///
///     全フォロワーのユーザタイムラインを取得
///
///         製造 : Retar.jp   
///         Ver 1.00  2019/11/06    初版
///
/// </summary>
////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Linq;                                     //参照で追加
using System.Windows.Forms;                            //参照で追加
using CoreTweet;                                       //追加してください。（CoreTweet・Nugetからパッケージを取得）
using CoreTweet.Core;
using NMeCab;                                          //追加してください。（NMeCabNetStandard・Nugetからパッケージを取得）                                        
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.Serialization.Json;

/// <summary>
///  メイン
/// </summary>
namespace TWTimeLineCapt
{
    class Program
    {
        /// <summary>
        ///設定の定義 class
        ///     同一実行Dirにsg.jsonを入れましょう
        /// </summary>
        static class Constants
        {
            public const bool sgOutConsoleDefault = false;                      //コンソール出力
            public const string sgFileNameDefault = "sg.json";                  //設定ファイル
            public const string sgTypeOfFriends = "Friends";                    //フォロー中　Friends
            public const string sgTypeOfFollowers = "Followers";                //フォロワー　Followers
            public const string sgID = "ID";                                    //ID出力
            public const string sgTWEET = "TWEET";                              //TWEET出力
            public const string sgJSON = "JSON";                                //JSON出力
            public const string sgJSONEXTENSION = ".json";                      //JSON拡張子
            public const string sgTRUEFILE = "TRUE.txt";                        //ID TRUE LIST ID
            public const string sgTRUELIST = "TRUELIST.csv";                    //ID TRUE LIST CSV
            public const string sgFALSEFILE = "FALSE.txt";                      //ID FALSE LIST ID
            public const string sgFALSELIST = "FALSELIST.csv";                  //ID FALSE LIST CSV
            public const string sgGRAYWORDLIST = "GRAYWORDLIST.csv";            //Gray Word List
            public const string sgGRAYWORDSTATICS = "GRAYWORDSTATICS.csv";      //Gray Word Statics
        }

        /// <summary>
        ///怪しい単語のタイムラインClass
        /// </summary>
        public class GrayMan
        {
            //Id
            public long? ID;
            //SCREEN NAME
            public string SCREENNAME;
            //Word
            public string WORD;
        }

        /// <summary>
        ///設定  
        ///     同一Dirにsg.jsonを入れましょう
        /// </summary>
        public class SG_JSON
        {
            //Consumer API keys (API key)
            //https://developer.twitter.com/en/docs/basics/authentication/api-reference/request_token
            public string twConsumerKey { get; set; }
            //Consumer API keys (API secret key)
            public string twConsumerSecret { get; set; }
            //Access token & access token secret  (Access token)
            public string twAccessToken { get; set; }
            //Access token & access token secret  (Access token secret)
            public string twAccessSecret { get; set; }
            //出力ディレクトリ
            public string twOutSubDir { get; set; }
            //取得回数  5000フォロワーにつき1回
            //https://developer.twitter.com/en/docs/accounts-and-users/follow-search-get-users/api-reference/get-friends-ids
            public int twFriendsIdsCount { get; set; }
            //取得ID数 MAX 5000
            public string twFriendsIdsParm_count { get; set; }
            //スクリーン名
            public string twFriendsIdsParm_screen_name { get; set; }
            //カーソル位置
            public string twFriendsIdsParm_cursor { get; set; }
            //出力ファイル
            public string twFriendsIdsOutFileName { get; set; }
            //出力ファイルの出力フラグ
            public bool twFriendsIdsOutFileOut { get; set; }
            //フォロワーとフォロー　種別 Followers / Friends
            public string twFriendsIdsTypeOfFollows { get; set; }
            //フォロワーとフォロー　Net(true)/File(false) 取得 スイッチ
            public bool twFriendsIdsGets { get; set; }
            //
            //取得タイムライン数
            public string twTimeLineCounts { get; set; }
            //Sleep時間
            public int twTimeLineSleepTime { get; set; }
            //失敗したユーザの一覧 タイムランなし、鍵ユーザ、削除ユーザの一覧
            public string twTimeLineFailedUsersFile { get; set; }
            //ユーザタイムランのCSV
            public string twTimeLineUsersFile { get; set; }
            //ユーザタイムランのJSON Dir
            public string twTimeLineUsersJSONDir { get; set; }
            //ユーザタイムラン Net(true)/File(false) 取得 スイッチ
            public bool twTimeLineGets { get; set; }
            //Tweets Counter つぶやきが一定数以下のアカウントを抽出
            public string twAnalysisCounter { get; set; }
            //Tweets Counter つぶやきが一定数以下のアカウントを抽出 Counter
            public int twAnalysisCounterCount { get; set; }
            ///Tweets Latest Filter X日以内にTweetしている
            public string twLatestFilter { get; set; }
            ///Tweets Latest Filter X日以内にTweetしている Counter
            public int twLatestFilterCounter { get; set; }
            //
            //Tweets Counter 特定用語の抽出
            public string twAnalysisWord { get; set; }
            //Tweets Counter GetIpadicDir 辞書ディレクトリ
            public string twGetIpadicDir { get; set; }
            //Tweets Counter GetGrayWords つぶやきワード
            public List<string> twGetGrayWords { get; set; }
            //Tweets Counter 特定用語の勘定
            public string twAnalysisWordCounter { get; set; }
            //Tweets Counter 特定用語の勘定ファイル
            public string twWordCounter { get; set; }
            //Tweets Counter 特定用語の名詞勘定ファイル
            public string twWordCounterNoun { get; set; }
            //
            //入力DIR
            public string sgINDIR { get; set; }
            //出力DIR
            public string sgOUTDIR { get; set; }
            //ファイルオリジナル部分サフィックス
            public string sgORGSUFFIX { get; set; }
            //ファイル共通部分サフィックス
            public string sgCOMMONSUFFIX { get; set; }
            //ファイルマージファイル
            public string sgMERGESUFFIX { get; set; }
        }

        /// <summary>
        ///Main
        /// </summary>
        static void Main(string[] args)
        {
            DateTime dtstart = DateTime.Now;

            ///SG読み込み
            string sgFileName = Constants.sgFileNameDefault;
            if (args.Length > 0) { sgFileName = args[0]; }
            //ファイルの存在チェック
            SG_JSON sgjson = new SG_JSON();
            if (System.IO.File.Exists(sgFileName))
            {
                sgjson = ReadJson(sgFileName);

                ////シリアライザ
                //DataContractJsonSerializer sgjs = new DataContractJsonSerializer(typeof(SG_JSON));
                ////ファイルストリーム・オープン
                //FileStream sgfs = new FileStream(sgFileName, FileMode.Open);
                ////JSONオブジェクトに設定
                //sgjson = (SG_JSON)sgjs.ReadObject(sgfs);
                ////ファイルストリーム・クローズ
                //sgfs.Close();
            }
            else
            {
                MessageBox.Show("'" + sgFileName + "'がありません。終了");
                Environment.Exit(0);    //異常終了
            }

            ///IDをTwitterから取得/書き込み   GET friends/ids / GET followers/ids
            List<long> twUserIds = new List<long>();
            twUserIds = TwitterIds(sgjson);

            ///各IDのTimeLine読み込み
            List<CoreTweet.Status> getResponsesUserTimeline = new List<CoreTweet.Status>();
            getResponsesUserTimeline = GetTimeLine(sgjson, twUserIds);

            ///Tweets Latest Filter 20日以内にTweetしている
            LatestFilter(sgjson, getResponsesUserTimeline);

            ///Tweets Counter つぶやきが一定数以下のアカウントを抽出
            AnalysisCounter(sgjson, getResponsesUserTimeline);

            ///用語勘定
            WordCounter(sgjson, getResponsesUserTimeline);

            ///用語抽出
            WordPicker(sgjson, getResponsesUserTimeline);

            ///終了処置
            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine($"{dtstart}  => {DateTime.Now}");
            Console.WriteLine("\n処理終了 : キー入力");
            Console.ReadKey();
        }

        /// <summary>
        ///ユーザごとのタイムラインを取得する 
        /// </summary>
        public static List<CoreTweet.Status> GetTimeLine(SG_JSON sgjson, List<long> twUserIds)
        {
            ///取得 Net(true)/File(false) 取得 スイッチ
            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine("タイムライン : 開始");
            List<CoreTweet.Status> getResponsesUserTimeline = new List<CoreTweet.Status>();
            if (sgjson.twTimeLineGets)
            {
                Console.WriteLine($"タイムライン : ネット取得開始");
                //twTimeLineGets
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                List<string> failedUsers = new List<string>();
                int lineCounter = 1;

                ///タイムライン読み込み
                foreach (long uidlong in twUserIds)
                {
                    ///認証
                    Tokens tokens = Tokens.Create(
                        sgjson.twConsumerKey,
                        sgjson.twConsumerSecret,
                        sgjson.twAccessToken,
                        sgjson.twAccessSecret
                        );

                    ///パラメータ
                    /// https://developer.twitter.com/en/docs/tweets/timelines/api-reference/get-statuses-user_timeline
                    var parm = new Dictionary<string, object>();                //条件指定用Dictionary
                    parm["count"] = sgjson.twTimeLineCounts;                    //取得数
                    parm["user_id"] = uidlong.ToString();                       //取得したいユーザーID
                    parm["trim_user"] = false;                                  //ユーザのTweetだけにする
                    parm["exclude_replies"] = false;                            //ユーザのリプライを排除
                    parm["include_rts"] = true;                                //ユーザRetweetをすべてとってくる

                    try
                    {
                        ListedResponse<CoreTweet.Status> utl = tokens.Statuses.UserTimeline(parm);
                        if (utl.Count == 0)
                        {
                            ///タイムランが読み込めないとき（タイムラン0）
                            failedUsers.Add(uidlong.ToString());
                        }
                        else
                        {
                            string sn = "";
                            foreach (var u in utl)
                            {
                                sn = u.User.ScreenName;
                                getResponsesUserTimeline.Add(u);
                            }
                            Console.WriteLine($"{String.Format("{0:000000}", lineCounter)} :      Id : {String.Format("{0, 20}", uidlong)} : Tweets : {String.Format("{0, 2}", utl.Count)} : SCREENNAME : {sn}");
                        }
                    }
                    catch
                    {
                        ///タイムランが読み込めないとき（鍵もしくは削除されたユーザ）
                        Console.WriteLine($"{String.Format("{0:000000}", lineCounter)} : Fail Id : {String.Format("{0, 20}", uidlong)}");
                        failedUsers.Add(uidlong.ToString());
                    }

                    ///スリープ
                    ///待てる限り長めに
                    Thread.Sleep(sgjson.twTimeLineSleepTime);
                    lineCounter++;
                }

                ///失敗したユーザの書き込み(IDのみ)
                ///優先して削除していいはずです。
                if (failedUsers.Count > 0)
                {
                    Console.WriteLine("-----------------------------------------------------------------");
                    Console.WriteLine("失敗したユーザ(IDのみ) : 書き込み開始");
                    var fuid = Directory.GetCurrentDirectory() + "\\" + sgjson.twOutSubDir + "\\" + sgjson.twTimeLineFailedUsersFile;
                    using (StreamWriter writer = new StreamWriter(fuid, false, Encoding.Unicode))
                    {
                        foreach (var item in failedUsers)
                        {
                            Console.WriteLine($"失敗したユーザ(IDのみ) : {String.Format("{0, 20}", item)}");
                            writer.WriteLine($"{item}");
                        }
                    }
                    Console.WriteLine("失敗したユーザ(IDのみ) : 書き込み終了");
                }

                ///CSVタイムライン書き込み
                WriteCSV(sgjson, getResponsesUserTimeline);

                ///JSONタイムライン書き込み
                if (getResponsesUserTimeline.Count > 0)
                {
                    Console.WriteLine("-----------------------------------------------------------------");
                    Console.WriteLine($"JSON書き込み : 書き込み開始");
                    var jtuid = Directory.GetCurrentDirectory() + "\\" + sgjson.twOutSubDir + "\\" + sgjson.twTimeLineUsersJSONDir;
                    Directory.CreateDirectory(jtuid);
                    foreach (CoreTweet.Status item in getResponsesUserTimeline)
                    {
                        var jsonData = JsonConvert.SerializeObject(item);
                        Console.WriteLine($"JSON書き込み : {String.Format("{0, 20}", item.User.ScreenName)} : {item.User.Id + "_" + item.Id + ".json"}");
                        using (StreamWriter writer = new StreamWriter(
                            jtuid + "\\"
                            + item.User.Id + "_" + item.Id + ".json"
                            , false, Encoding.Unicode))
                        {
                            writer.Write(jsonData);
                        }
                    }
                    Console.WriteLine($"JSON書き込み : 書き込み終了");
                }

                Console.WriteLine($"タイムライン : レコード : {getResponsesUserTimeline.Count}");
                Console.WriteLine($"タイムライン : ネット取得完了");
            }
            else
            {
                var jtuid = Directory.GetCurrentDirectory() + "\\" + sgjson.twOutSubDir + "\\" + sgjson.twTimeLineUsersJSONDir;
                Directory.CreateDirectory(jtuid);
                try
                {
                    ///Fileから取得
                    Console.WriteLine($"タイムライン : ファイル読み込み開始");
                    System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(jtuid + "\\");
                    IEnumerable<System.IO.FileInfo> bFiles =
                                di.EnumerateFiles("*", System.IO.SearchOption.AllDirectories);
                    int lineCounter = 1;
                    foreach (System.IO.FileInfo bF in bFiles)
                    {
                        try
                        {
                            using (var bFs = new StreamReader(bF.FullName, Encoding.Unicode))
                            {
                                var data = bFs.ReadToEnd();
                                CoreTweet.Status bS = JsonConvert.DeserializeObject<CoreTweet.Status>(data);
                                if ((from x in twUserIds select x).Where(y => y == bS.User.Id).Count() > 0)
                                {
                                    getResponsesUserTimeline.Add(bS);
                                }
                                if (lineCounter % 1000 == 0)
                                {
                                    Console.WriteLine($"{String.Format("{0:0000000}", lineCounter)} : Read ");
                                }
                                //Console.WriteLine($"{String.Format("{0:0000000}", lineCounter)} : Read      {Path.GetFileName(bF.FullName)}");
                            }
                        }
                        catch
                        {
                            Console.WriteLine($"{String.Format("{0:0000000}", lineCounter)} : Read Fail {Path.GetFileName(bF.FullName)}");
                        }
                        lineCounter++;
                    }
                    Console.WriteLine($"{String.Format("{0:0000000}", lineCounter)} : Read ");

                    ///CSVタイムライン書き込み
                    WriteCSV(sgjson, getResponsesUserTimeline);
                }
                catch
                {
                    ///タイムラインファイル読み込みエラー
                    Console.WriteLine("-----------------------------------------------------------------");
                    Console.WriteLine("タイムライン取得 : 読み込み失敗 : {0}", jtuid);
                    Console.WriteLine("タイムライン取得 : 異常終了");
                    Console.ReadKey();
                    Environment.Exit(0);                    //プログラム終了
                }

                Console.WriteLine($"タイムライン : レコード : {getResponsesUserTimeline.Count}");
                Console.WriteLine($"タイムライン : ファイル読み込み完了");
            }

            ///タイムラン List返却
            Console.WriteLine("タイムライン : 完了");
            return getResponsesUserTimeline;
        }

        /// <summary>
        ///SG読み込み
        /// </summary>
        public static SG_JSON ReadJson(string sgFileName)
        {
            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine("SG読み込み : 開始");

            SG_JSON sgjson = new SG_JSON();
            try
            {
                if (System.IO.File.Exists(sgFileName))
                {
                    ///設定読み込み
                    using (var bFs = new StreamReader(sgFileName, Encoding.Unicode))
                    {
                        var data = bFs.ReadToEnd();
                        sgjson = JsonConvert.DeserializeObject<SG_JSON>(data);
                    }
                }
                else
                {
                    MessageBox.Show("'" + sgFileName + "'がありません。終了");
                    Environment.Exit(0);                    //プログラム終了
                }
                Console.WriteLine("SG読み込み : 完了");
            }
            catch
            {
                Console.WriteLine("SG読み込み : 失敗 : UNICODE文字コードチェックを！");
                Environment.Exit(0);                    //プログラム終了
                Console.ReadKey();
            }

            ///設定
            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine(">>> Setting <<<");
            Console.WriteLine($"twConsumerKey                   : {sgjson.twConsumerKey}");
            Console.WriteLine($"twConsumerSecret                : {sgjson.twConsumerSecret}");
            Console.WriteLine($"twAccessToken                   : {sgjson.twAccessToken}");
            Console.WriteLine($"twAccessSecret                  : {sgjson.twAccessSecret}");
            Console.WriteLine($"twOutSubDir                     : {sgjson.twOutSubDir}");
            Console.WriteLine($"twFriendsIdsCount               : {sgjson.twFriendsIdsCount}");
            Console.WriteLine($"twFriendsIdsParm_count          : {sgjson.twFriendsIdsParm_count}");
            Console.WriteLine($"twFriendsIdsParm_cursor         : {sgjson.twFriendsIdsParm_cursor}");
            Console.WriteLine($"twFriendsIdsOutFileName         : {sgjson.twFriendsIdsOutFileName}");
            Console.WriteLine($"twFriendsIdsOutFileOut          : {sgjson.twFriendsIdsOutFileOut}");
            Console.WriteLine($"twFriendsIdsTypeOfFollows       : {sgjson.twFriendsIdsTypeOfFollows}");
            Console.WriteLine($"twFriendsIdsGets                : {sgjson.twFriendsIdsGets}");
            Console.WriteLine($"twTimeLineCounts                : {sgjson.twTimeLineCounts}");
            Console.WriteLine($"twTimeLineSleepTime             : {sgjson.twTimeLineSleepTime}");
            Console.WriteLine($"twTimeLineFailedUsersFile       : {sgjson.twTimeLineFailedUsersFile}");
            Console.WriteLine($"twTimeLineUsersFile             : {sgjson.twTimeLineUsersFile}");
            Console.WriteLine($"twTimeLineUsersJSONDir          : {sgjson.twTimeLineUsersJSONDir}");
            Console.WriteLine($"twTimeLineGets                  : {sgjson.twTimeLineGets}");
            Console.WriteLine($"twAnalysisCounter               : {sgjson.twAnalysisCounter}");
            Console.WriteLine($"twAnalysisCounterCount          : {sgjson.twAnalysisCounterCount}");
            Console.WriteLine($"twLatestFilter                  : {sgjson.twLatestFilter}");
            Console.WriteLine($"twLatestFilterCounter           : {sgjson.twLatestFilterCounter}");
            Console.WriteLine($"twGetIpadicDir                  : {sgjson.twGetIpadicDir}");
            Console.WriteLine($"twAnalysisWord                  : {sgjson.twAnalysisWord}");
            Console.Write($"twGetGrayWords                  : ");
            sgjson.twGetGrayWords.ForEach(i => Console.Write("{0}\t", i));
            Console.Write("\n");
            Console.WriteLine($"twAnalysisWordCounter           : {sgjson.twAnalysisWordCounter}");
            Console.WriteLine($"twWordCounter                   : {sgjson.twWordCounter}");
            Console.WriteLine($"twWordCounterNoun               : {sgjson.twWordCounterNoun}");     //機能未追加
            Console.WriteLine($"sgINDIR                         : {sgjson.sgINDIR}");
            Console.WriteLine($"sgOUTDIR                        : {sgjson.sgOUTDIR }");
            Console.WriteLine($"sgORGSUFFIX                     : {sgjson.sgORGSUFFIX }");
            Console.WriteLine($"sgCOMMONSUFFIX                  : {sgjson.sgCOMMONSUFFIX }");
            Console.WriteLine($"sgMERGESUFFIX                   : {sgjson.sgMERGESUFFIX }");

            return sgjson;
        }

        /// <summary>
        ///用語勘定
        /// </summary>
        public static void WordCounter(SG_JSON sgjson, List<CoreTweet.Status> getResponsesUserTimeline)
        {
            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine($"用語勘定 : 開始");

            ///NMeCabで分解
            MeCabParam mPara = new MeCabParam();
            mPara.DicDir = Directory.GetCurrentDirectory() + "\\" + sgjson.twGetIpadicDir;
            MeCabTagger mTagger = MeCabTagger.Create(mPara);

            ///出力ディレクトリ生成
            var awdir = Directory.GetCurrentDirectory() + "\\"
                   + sgjson.twOutSubDir + "\\"
                   + sgjson.twAnalysisWordCounter;
            //dictionary
            Dictionary<string, int> wordTable = new Dictionary<string, int>();
            //Dictionary<string, int> wordTableNoun = new Dictionary<string, int>();    //開発中止

            try
            {
                Directory.CreateDirectory(awdir);
                Console.WriteLine($"用語勘定 : MKDIR : {awdir}");

                ///用語分析        
                var ftwAnalysisWordCounter = awdir + "\\" + sgjson.twWordCounter;
                using (StreamWriter wAnalysisWordCounter = new StreamWriter(ftwAnalysisWordCounter, false, Encoding.Unicode))
                {
                    foreach (var ritem in getResponsesUserTimeline)
                    {
                        var rText = System.Text.RegularExpressions.Regex.Replace(ritem.Text, @"[\t]+", " ");
                        rText = System.Text.RegularExpressions.Regex.Replace(rText, @"[\n]+", " ");
                        rText = System.Text.RegularExpressions.Regex.Replace(rText, @"[\r]+", " ");
                        rText = System.Text.RegularExpressions.Regex.Replace(rText, @"[\0]+", " ");

                        MeCabNode node = mTagger.ParseToNode(rText);
                        string aw = "";

                        if (node != null) { node = node.Next; }
                        while (node != null)
                        {
                            if (wordTable.ContainsKey(node.Surface))
                            {
                                var wCount = wordTable[node.Surface];
                                wCount++;
                                wordTable.Remove(node.Surface);
                                wordTable.Add(node.Surface, wCount);
                            }
                            else
                            {
                                aw = node.Surface;
                                wordTable.Add(node.Surface, 1);
                            }
                            node = node.Next;
                        }
                    }

                    ///書き出し
                    wAnalysisWordCounter.WriteLine($"Word,Count,Part01,Part02");
                    foreach (var item in wordTable)
                    {
                        MeCabNode node = mTagger.ParseToNode(item.Key);
                        if (node != null) { node = node.Next; }
                        while (node != null)
                        {
                            if (node.Feature != null)
                            {
                                wAnalysisWordCounter.WriteLine($"\"{item.Key}\",{item.Value},{node.Feature}");
                            }
                            node = node.Next;
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine($"用語勘定 : ERROR");
            }
            Console.WriteLine($"用語勘定 : 完了");
        }

        /// <summary>
        ///用語抽出
        /// </summary>
        public static void WordPicker(SG_JSON sgjson, List<CoreTweet.Status> getResponsesUserTimeline)
        {
            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine($"用語抽出 : 開始");

            ///NMeCabで分解
            MeCabParam mPara = new MeCabParam();
            mPara.DicDir = Directory.GetCurrentDirectory() + "\\" + sgjson.twGetIpadicDir;
            MeCabTagger mTagger = MeCabTagger.Create(mPara);

            ///出力ディレクトリ生成
            List<CoreTweet.Status> GrayTweetsLists = new List<CoreTweet.Status>();
            var awdir = Directory.GetCurrentDirectory() + "\\"
                   + sgjson.twOutSubDir + "\\"
                   + sgjson.twAnalysisWord;
            List<GrayMan> gMans = new List<GrayMan>();   //怪しい垢
            try
            {
                Directory.CreateDirectory(awdir);
                Console.WriteLine($"用語抽出 : MKDIR : {awdir}");

                //int ct = 0;
                foreach (var item in sgjson.twGetGrayWords)
                {
                    var targetdir = awdir + "\\" + item;
                    Directory.CreateDirectory(targetdir);
                    Console.WriteLine($"用語抽出 : Run : {item}");

                    var targetdirTRUEFILE = targetdir + "\\" + Constants.sgTRUEFILE;
                    var targetdirTRUELIST = targetdir + "\\" + Constants.sgTRUELIST;

                    using (StreamWriter wtargetdirTRUEFILE = new StreamWriter(targetdirTRUEFILE, false, Encoding.Unicode))
                    using (StreamWriter wtargetdirTRUELIST = new StreamWriter(targetdirTRUELIST, false, Encoding.Unicode))
                    {
                        wtargetdirTRUELIST.WriteLine($"UserTimeRun,Id,CreatedAt,CreatedAt.LocalDateTime,User.Id,ScreenName,RetweetedStatus,Source,Text");

                        foreach (var ritem in getResponsesUserTimeline)
                        {
                            var rText = System.Text.RegularExpressions.Regex.Replace(ritem.Text, @"[\t]+", "");
                            rText = System.Text.RegularExpressions.Regex.Replace(rText, @"[\n]+", "");
                            rText = System.Text.RegularExpressions.Regex.Replace(rText, @"[\r]+", "");
                            MeCabNode node = mTagger.ParseToNode(rText);

                            while (node != null)
                            {
                                if (Regex.IsMatch(node.Surface, item))
                                {
                                    GrayMan gm = new GrayMan();
                                    gm.ID = ritem.User.Id;
                                    gm.SCREENNAME = ritem.User.ScreenName;
                                    gm.WORD = item;
                                    gMans.Add(gm);

                                    //Console.WriteLine($"用語抽出 : {item} : {ritem.User.Id}");
                                    GrayTweetsLists.Add(ritem);
                                    wtargetdirTRUEFILE.WriteLine(ritem.User.Id);
                                    wtargetdirTRUELIST.WriteLine(
                                            $"UserTimeRun," +
                                            $"\"{ritem.Id}\"," +
                                            $"\"{ritem.CreatedAt}\"," +
                                            $"\"{ritem.CreatedAt.LocalDateTime}\"," +
                                            $"\"{ritem.User.Id}\"," +
                                            $"\"{ritem.User.ScreenName}\"," +
                                            $"\"{ritem.Language}\"," +
                                            $"\"{ritem.RetweetedStatus}\"," +
                                            $"\"{HtmlTagDelete(ritem.Source)}\"," +
                                            $"\"{rText}" +
                                            $"\""
                                            );
                                    break;
                                }
                                node = node.Next;
                            }
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine("-----------------------------------------------------------------");
                Console.WriteLine("用語抽出 : 異常終了");
            }

            ///グレーワードの統計
            using (StreamWriter writer = new StreamWriter(awdir + "\\" + Constants.sgGRAYWORDLIST, false, Encoding.Unicode))
            {
                writer.WriteLine($"UserTimeRun,Id,CreatedAt,CreatedAt.LocalDateTime,User.Id,ScreenName,Language,RetweetedStatus,Source,Text");
                foreach (var item in GrayTweetsLists)
                {
                    var rText = System.Text.RegularExpressions.Regex.Replace(item.Text, @"[\t]+", "");
                    rText = System.Text.RegularExpressions.Regex.Replace(rText, @"[\n]+", "");
                    rText = System.Text.RegularExpressions.Regex.Replace(rText, @"[\r]+", "");
                    writer.WriteLine(
                            $"UserTimeRun," +
                            $"\"{item.Id}\"," +
                            $"\"{item.CreatedAt}\"," +
                            $"\"{item.CreatedAt.LocalDateTime}\"," +
                            $"\"{item.User.Id}\"," +
                            $"\"{item.User.ScreenName}\"," +
                            $"\"{item.Language}\"," +
                            $"\"{item.RetweetedStatus}\"," +
                            $"\"{HtmlTagDelete(item.Source)}\"," +
                            $"\"{rText}" +
                            $"\""
                            );
                }
            }

            ///グレーワードの集計
            using (StreamWriter writer = new StreamWriter(awdir + "\\" + Constants.sgGRAYWORDSTATICS, false, Encoding.Unicode))
            {
                var ids = (from x in gMans select x.ID).Distinct().ToList();    //IDのユニーク

                writer.Write($"ID,SCREENNAME,");
                foreach (var ggitem in sgjson.twGetGrayWords)
                {
                    writer.Write($"{ggitem},");
                }
                writer.Write($"SUM,URL\n");

                foreach (var iditem in ids)
                {
                    var gu = (from x in gMans select x).Where(y => y.ID == iditem);
                    var sname = gu.Select(x => x.SCREENNAME).FirstOrDefault();
                    writer.Write($"{iditem},{sname},");
                    int gwordsum = 0;
                    foreach (var ggitem in sgjson.twGetGrayWords)
                    {
                        var gwordcount = (from x in gu select x).Where(y => y.WORD == ggitem).Count();
                        writer.Write($"{gwordcount},");
                        gwordsum = gwordsum + gwordcount;
                    }
                    writer.Write($"{gwordsum}," + $"\"https://twitter.com/" + $"{sname}" + $"/with_replies\"\n");
                }
            }
            Console.WriteLine($"用語抽出 : 完了");
        }

        /// <summary>
        ///Tweets Latest Filter ??日以内にTweetしている
        /// </summary>
        public static void LatestFilter(SG_JSON sgjson, List<CoreTweet.Status> getResponsesUserTimeline)
        {
            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine($"Tweets Latest Filter : 開始");
            Console.WriteLine($"Tweets Latest Filter : 閾値 : {sgjson.twLatestFilterCounter}");

            var lfdir = Directory.GetCurrentDirectory() + "\\"
                    + sgjson.twOutSubDir + "\\"
                    + sgjson.twLatestFilter;
            Directory.CreateDirectory(lfdir);
            var lfsgTRUEFILE = lfdir + "\\" + Constants.sgTRUEFILE;
            var lfsgTRUELIST = lfdir + "\\" + Constants.sgTRUELIST;
            var lfsgFALSEFILE = lfdir + "\\" + Constants.sgFALSEFILE;
            var lfsgFALSELIST = lfdir + "\\" + Constants.sgFALSELIST;

            ///ユニークなScreenName抽出
            var DistinctUID = (from x in getResponsesUserTimeline select x.User.Id).Distinct().ToList();

            ///まとめて4ファイルオープン
            using (StreamWriter wlfsgTRUEFILE = new StreamWriter(lfsgTRUEFILE, false, Encoding.Unicode))
            using (StreamWriter wlfsgTRUELIST = new StreamWriter(lfsgTRUELIST, false, Encoding.Unicode))
            using (StreamWriter wlfsgFALSEFILE = new StreamWriter(lfsgFALSEFILE, false, Encoding.Unicode))
            using (StreamWriter wlfsgFALSELIST = new StreamWriter(lfsgFALSELIST, false, Encoding.Unicode))
            {
                wlfsgTRUELIST.WriteLine($"UserTimeRun,Id,CreatedAt,CreatedAt.LocalDateTime,User.Id,ScreenName,Language,RetweetedStatus,Source,Text");
                wlfsgFALSELIST.WriteLine($"UserTimeRun,Id,CreatedAt,CreatedAt.LocalDateTime,User.Id,ScreenName,Language,RetweetedStatus,Source,Text");
                DateTime dtimeDiv = DateTime.Now.AddDays((sgjson.twLatestFilterCounter) * -1);
                foreach (var item in DistinctUID)
                {
                    var dtCount = (from x in getResponsesUserTimeline select x).Where(y => y.User.Id == item).Where(y => y.CreatedAt.LocalDateTime <= dtimeDiv).Count();
                    if (dtCount == 0)
                    {
                        var llfsgTRUELIST = (from x in getResponsesUserTimeline where x.User.Id == item select x);
                        wlfsgTRUEFILE.WriteLine($"{item}");
                        Console.WriteLine($"Tweets Counter : True  : {item}");
                        foreach (var litem in llfsgTRUELIST)
                        {
                            var rText = Regex.Replace(litem.Text, @"[\t]+", "");
                            rText = Regex.Replace(rText, @"[\n]+", "");
                            rText = Regex.Replace(rText, @"[\r]+", "");
                            wlfsgTRUELIST.WriteLine(
                                $"UserTimeRun," +
                                $"\"{litem.Id}\"," +
                                $"\"{litem.CreatedAt}\"," +
                                $"\"{litem.CreatedAt.LocalDateTime}\"," +
                                $"\"{litem.User.Id}\"," +
                                $"\"{litem.User.ScreenName}\"," +
                                $"\"{litem.Language}\"," +
                                $"\"{litem.RetweetedStatus}\"," +
                                $"\"{HtmlTagDelete(litem.Source)}\"," +
                                $"\"{rText}" +
                                $"\""
                                );
                        }
                    }
                    else
                    {
                        var llfsgFALSELIST = (from x in getResponsesUserTimeline where x.User.Id == item select x);
                        wlfsgFALSEFILE.WriteLine($"{item}");
                        Console.WriteLine($"Tweets Counter : False : {item}");
                        foreach (var litem in llfsgFALSELIST)
                        {
                            var rText = Regex.Replace(litem.Text, @"[\t]+", "");
                            rText = Regex.Replace(rText, @"[\n]+", "");
                            rText = Regex.Replace(rText, @"[\r]+", "");
                            wlfsgFALSELIST.WriteLine(
                                $"UserTimeRun," +
                                $"\"{litem.Id}\"," +
                                $"\"{litem.CreatedAt}\"," +
                                $"\"{litem.CreatedAt.LocalDateTime}\"," +
                                $"\"{litem.User.Id}\"," +
                                $"\"{litem.User.ScreenName}\"," +
                                $"\"{litem.Language}\"," +
                                $"\"{litem.RetweetedStatus}\"," +
                                $"\"{HtmlTagDelete(litem.Source)}\"," +
                                $"\"{rText}" +
                                $"\""
                                );
                        }
                    }
                }
            }
            Console.WriteLine($"Tweets Latest Filter : 完了");
        }

        /// <summary>
        ///Tweets Counter つぶやきが一定数以下のアカウントを抽出
        /// </summary>
        public static void AnalysisCounter(SG_JSON sgjson, List<CoreTweet.Status> getResponsesUserTimeline)
        {
            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine($"Tweets Counter : 開始");
            Console.WriteLine($"Tweets Counter : 閾値 : {sgjson.twAnalysisCounterCount}");

            var anadir = Directory.GetCurrentDirectory() + "\\" + sgjson.twOutSubDir + "\\" + sgjson.twAnalysisCounter;
            Directory.CreateDirectory(anadir);
            var anasgTRUEFILE = anadir + "\\" + Constants.sgTRUEFILE;
            var anasgTRUELIST = anadir + "\\" + Constants.sgTRUELIST;
            var anasgFALSEFILE = anadir + "\\" + Constants.sgFALSEFILE;
            var anasgFALSELIST = anadir + "\\" + Constants.sgFALSELIST;

            ///ユニークなScreenName抽出
            var DistinctUID = (from x in getResponsesUserTimeline select x.User.Id).Distinct().ToList();

            ///まとめて4ファイルオープン
            using (StreamWriter wanasgTRUEFILE = new StreamWriter(anasgTRUEFILE, false, Encoding.Unicode))
            using (StreamWriter wanasgTRUELIST = new StreamWriter(anasgTRUELIST, false, Encoding.Unicode))
            using (StreamWriter wanasgFALSEFILE = new StreamWriter(anasgFALSEFILE, false, Encoding.Unicode))
            using (StreamWriter wanasgFALSELIST = new StreamWriter(anasgFALSELIST, false, Encoding.Unicode))
            {
                wanasgTRUELIST.WriteLine($"UserTimeRun,Id,CreatedAt,CreatedAt.LocalDateTime,User.Id,ScreenName,Language,RetweetedStatus,Source,Text");
                wanasgFALSELIST.WriteLine($"UserTimeRun,Id,CreatedAt,CreatedAt.LocalDateTime,User.Id,ScreenName,Language,RetweetedStatus,Source,Text");

                foreach (var item in DistinctUID)
                {
                    ///つぶやき数    満たすものTrue / 満たさないものFalse
                    var acCount = (from x in getResponsesUserTimeline select x).Where(y => y.User.Id == item).Count();
                    if (acCount >= sgjson.twAnalysisCounterCount)
                    {
                        var lanasgTRUELIST = (from x in getResponsesUserTimeline where x.User.Id == item select x);
                        wanasgTRUEFILE.WriteLine($"{item}");
                        Console.WriteLine($"Tweets Counter : True  : {item}");
                        foreach (var litem in lanasgTRUELIST)
                        {
                            var rText = Regex.Replace(litem.Text, @"[\t]+", "");
                            rText = Regex.Replace(rText, @"[\n]+", "");
                            rText = Regex.Replace(rText, @"[\r]+", "");
                            wanasgTRUELIST.WriteLine(
                                $"UserTimeRun," +
                                $"\"{litem.Id}\"," +
                                $"\"{litem.CreatedAt}\"," +
                                $"\"{litem.CreatedAt.LocalDateTime}\"," +
                                $"\"{litem.User.Id}\"," +
                                $"\"{litem.User.ScreenName}\"," +
                                $"\"{litem.Language}\"," +
                                $"\"{litem.RetweetedStatus}\"," +
                                $"\"{HtmlTagDelete(litem.Source)}\"," +
                                $"\"{rText}" +
                                $"\""
                                );
                        }
                    }
                    else
                    {
                        var lanasgFALSELIST = (from x in getResponsesUserTimeline where x.User.Id == item select x);
                        wanasgFALSEFILE.WriteLine($"{item}");
                        Console.WriteLine($"Tweets Counter : False : {item}");
                        foreach (var litem in lanasgFALSELIST)
                        {
                            var rText = Regex.Replace(litem.Text, @"[\t]+", "");
                            rText = Regex.Replace(rText, @"[\n]+", "");
                            rText = Regex.Replace(rText, @"[\r]+", "");
                            wanasgFALSELIST.WriteLine(
                                $"UserTimeRun," +
                                $"\"{litem.Id}\"," +
                                $"\"{litem.CreatedAt}\"," +
                                $"\"{litem.CreatedAt.LocalDateTime}\"," +
                                $"\"{litem.User.Id}\"," +
                                $"\"{litem.User.ScreenName}\"," +
                                $"\"{litem.Language}\"," +
                                $"\"{litem.RetweetedStatus}\"," +
                                $"\"{HtmlTagDelete(litem.Source)}\"," +
                                $"\"{rText}" +
                                $"\""
                                );
                        }
                    }
                }
            }
            Console.WriteLine($"Tweets Counter : 完了");
        }

        /// <summary>
        ///ID取得
        /// </summary>
        public static List<long> TwitterIds(SG_JSON sgjson)
        {
            ///出力ディレクトリ作成
            var ttwoutd = Directory.GetCurrentDirectory() + "\\" + sgjson.twOutSubDir;
            Directory.CreateDirectory(ttwoutd);
            var inDir = Directory.GetCurrentDirectory() + "\\" + sgjson.twOutSubDir + "\\" + sgjson.sgINDIR;
            var outDir = Directory.GetCurrentDirectory() + "\\" + sgjson.twOutSubDir + "\\" + sgjson.sgOUTDIR;
            Directory.CreateDirectory(inDir);
            Directory.CreateDirectory(outDir);

            ///取得 Net(true)/File(false) 取得 スイッチ
            List<long> twUserIds = new List<long>();
            if (sgjson.twFriendsIdsGets)
            {
                ///Twitterから取得
                try
                {
                    ///認証
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    Tokens tokens = Tokens.Create(
                        sgjson.twConsumerKey,
                        sgjson.twConsumerSecret,
                        sgjson.twAccessToken,
                        sgjson.twAccessSecret
                        );

                    ///パラメータ
                    var parm = new Dictionary<string, object>();                    //条件指定用Dictionary
                    parm["count"] = sgjson.twFriendsIdsParm_count;                  //取得数
                    parm["screen_name"] = sgjson.twFriendsIdsParm_screen_name;      //取得したいユーザーID
                    if (sgjson.twFriendsIdsParm_cursor != "")
                    {
                        parm["cursor"] = sgjson.twFriendsIdsParm_cursor;            //設定があればカーソル設定
                    }

                    ///フォローリストの読み込み
                    Console.WriteLine("-----------------------------------------------------------------");
                    Console.WriteLine("Id取得 : ネット取得開始");
                    for (var twfIC = 0; twfIC < sgjson.twFriendsIdsCount; twfIC++)
                    {
                        ///取得対象で分類
                        switch (sgjson.twFriendsIdsTypeOfFollows)
                        {
                            ///friends/idsを取得
                            case Constants.sgTypeOfFriends:
                                var fls = tokens.Friends.Ids(parm);
                                foreach (var f in fls)
                                {
                                    twUserIds.Add(f);
                                }
                                parm["cursor"] = fls.NextCursor;            //カーソル設定
                                Console.WriteLine("Id取得 : {0} : 次のカーソル : {1}", twfIC, fls.NextCursor);
                                break;
                            ///followers/idsを取得
                            case Constants.sgTypeOfFollowers:
                                var flst = tokens.Followers.Ids(parm);
                                foreach (var f in flst)
                                {
                                    twUserIds.Add(f);
                                }
                                parm["cursor"] = flst.NextCursor;           //カーソル設定
                                Console.WriteLine("Id取得 : {0} : 次のカーソル : {1}", twfIC, flst.NextCursor);
                                break;
                            ///異常終了
                            default:
                                MessageBox.Show(
                                    "異常終了 : sgjson.twFriendsIdsTypeOfFollows : "
                                    + sgjson.twFriendsIdsTypeOfFollows
                                    );
                                Environment.Exit(0);
                                break;
                        }
                    }
                    Console.WriteLine("Id取得 : ネット取得終了");
                }
                catch (TwitterException e)
                {
                    ///CoreTweetエラー
                    Console.WriteLine("-----------------------------------------------------------------");
                    Console.WriteLine("Id取得 : CoreTweet Error : {0}", e.Message);
                    Console.WriteLine("Id取得 : 異常終了");
                    Console.ReadKey();
                    Environment.Exit(0);                    //プログラム終了
                }
                catch (System.Net.WebException e)
                {
                    ///インターネット接続エラー
                    Console.WriteLine("-----------------------------------------------------------------");
                    Console.WriteLine("Id取得 : Internet Error : {0}", e.Message);
                    Console.WriteLine("Id取得 : 異常終了");
                    Console.ReadKey();
                    Environment.Exit(0);                    //プログラム終了
                }

                ///IDをファイルに書き込み
                try
                {
                    Console.WriteLine("-----------------------------------------------------------------");
                    Console.WriteLine("Id書き出し : 開始");
                    if (sgjson.twFriendsIdsOutFileOut)
                    {
                        ///出力ディレクトリ生成
                        var cdir = Directory.GetCurrentDirectory();
                        try
                        {
                            Console.WriteLine("Id書き出し : 出力ディレクトリ : 生成開始");
                            Console.WriteLine($"Id書き出し : {cdir}\\{sgjson.twOutSubDir}\\{sgjson.sgOUTDIR}");
                            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo($"{cdir}\\{sgjson.twOutSubDir}");
                            di.Create();
                            Console.WriteLine("Id書き出し : 出力ディレクトリ : 生成完了");
                        }
                        catch
                        {
                            Console.WriteLine("Id書き出し : 出力ディレクトリ : 生成失敗 : 異常終了");
                            Environment.Exit(0);                    //プログラム終了
                            Console.ReadKey();
                        }

                        ///StreamWriterオブジェクト生成で書き出し
                        Console.WriteLine($"Id書き出し : File ; {cdir}\\{sgjson.twOutSubDir}\\{sgjson.sgOUTDIR}\\{sgjson.twFriendsIdsOutFileName}");
                        using (StreamWriter writer = new StreamWriter(
                                    cdir + "\\" +
                                    sgjson.twOutSubDir + "\\" +
                                    sgjson.sgOUTDIR + "\\" +
                                    sgjson.twFriendsIdsOutFileName,
                                    false, Encoding.Unicode))
                        {
                            foreach (var sn in twUserIds)
                            {
                                writer.WriteLine(sn);
                            }
                        }
                        Console.WriteLine($"Id書き出し : File ; {cdir}\\{sgjson.twOutSubDir}\\{sgjson.sgINDIR}\\{sgjson.twFriendsIdsOutFileName}");
                        using (StreamWriter writer = new StreamWriter(
                                    cdir + "\\" +
                                    sgjson.twOutSubDir + "\\" +
                                    sgjson.sgINDIR + "\\" +
                                    sgjson.twFriendsIdsOutFileName,
                                    false, Encoding.Unicode))
                        {
                            foreach (var sn in twUserIds)
                            {
                                writer.WriteLine(sn);
                            }
                        }
                    }
                    Console.WriteLine("Id書き出し : 終了");
                }
                catch (Exception)
                {
                    Console.WriteLine("Id書き出し : 失敗");
                    Environment.Exit(0);                    //プログラム終了
                    Console.ReadKey();
                }
            }
            else
            {
                ///Fileから取得
                try
                {
                    Console.WriteLine("-----------------------------------------------------------------");
                    Console.WriteLine("Id取得 : ローカルファイル取得開始");
                    twUserIds = CONVReadFiles(sgjson);
                    Console.WriteLine("Id取得 : ローカルファイル取得完了");
                }
                catch
                {
                    ///IDファイル読み込みエラー
                    Console.WriteLine("-----------------------------------------------------------------");
                    Console.WriteLine($"Id取得 : 読み込み失敗 : 入力 :{inDir}");
                    Console.WriteLine($"Id取得 : 読み込み失敗 : 出力 :{outDir}");
                    Console.WriteLine("Id取得 : 異常終了");
                    Console.ReadKey();
                    Environment.Exit(0);                    //プログラム終了
                }
            }

            ///ID List返却
            return twUserIds;
        }

        /// <summary>
        ///ファイル読み込み
        /// </summary>
        public static List<long> CONVReadFiles(SG_JSON sgjson)
        {
            ///返却
            List<long> mergeList = new List<long>();
            List<long> commonList = new List<long>();

            ///設定
            var inDir = Directory.GetCurrentDirectory() + "\\" + sgjson.twOutSubDir + "\\" + sgjson.sgINDIR;
            var outDir = Directory.GetCurrentDirectory() + "\\" + sgjson.twOutSubDir + "\\" + sgjson.sgOUTDIR;
            Directory.CreateDirectory(inDir);
            Directory.CreateDirectory(outDir);

            string[] inFiles = System.IO.Directory.GetFiles(inDir, "*", System.IO.SearchOption.AllDirectories);
            if (inFiles.Count() == 0)
            {
                MessageBox.Show("入力ファイルがありません。終了");
                Environment.Exit(0);                    //プログラム終了
            }
            Dictionary<string, string> outFiles = new Dictionary<string, string>();
            foreach (var item in inFiles)
            {
                string front = System.IO.Path.GetFileNameWithoutExtension(item);
                string of = outDir + "\\" + front + sgjson.sgORGSUFFIX;
                outFiles.Add(item, of);

            }
            //COMMON
            string commonFile = outDir + "\\" + sgjson.sgCOMMONSUFFIX;
            //MERGE
            string mergeFile = outDir + "\\" + sgjson.sgMERGESUFFIX;

            Console.WriteLine($"入力DIR         : {inDir}");
            foreach (var item in inFiles)
            {
                Console.WriteLine($"入力FILE        : {item}");
            }
            Console.WriteLine($"出力DIR         : {outDir}");
            foreach (var item in outFiles)
            {
                Console.WriteLine($"出力FILE        : {item.Value}");
            }
            Console.WriteLine($"COMMON FILE     : {commonFile}");
            Console.WriteLine($"MERGE FILE      : {mergeFile}");

            ///MERGE
            foreach (var f in inFiles)
            {
                using (StreamReader reader = new StreamReader(f, Encoding.Unicode))
                {
                    List<string> mlb = new List<string>();
                    while (reader.EndOfStream == false)
                    {
                        mlb.Add(reader.ReadLine());
                        //mergeList.Add(long.Parse(reader.ReadLine()));
                    }
                    mergeList.AddRange(mlb.Distinct().OrderBy(x => x).ToList());    //Bug Fix 2019/11/11
                }
            }
            List<long> clistbef = new List<long>(mergeList);
            mergeList = (from x in mergeList select x).Distinct().ToList();
            using (StreamWriter writer = new StreamWriter(mergeFile, false, Encoding.Unicode))
            {
                foreach (var item in mergeList)
                {
                    writer.WriteLine(item);
                }
            }

            ///COMMON
            foreach (var item in mergeList)
            {
                var c = (from x in clistbef select x).Where(y => y == item).Count();
                if (c > 1)
                {
                    commonList.Add(item);
                }
            }
            using (StreamWriter writer = new StreamWriter(commonFile, false, Encoding.Unicode))
            {
                foreach (var item in commonList)
                {
                    writer.WriteLine(item);
                }
            }

            ///ORG  入力ファイル独自部分
            foreach (var f in outFiles)
            {
                var cl = new List<long>(commonList);
                var orgl = new List<long>();

                using (StreamReader reader = new StreamReader(f.Key, Encoding.Unicode))
                {
                    while (reader.EndOfStream == false)
                    {
                        orgl.Add(long.Parse(reader.ReadLine()));
                    }
                }
                foreach (var item in cl)
                {
                    orgl.Remove(item);
                }
                using (StreamWriter writer = new StreamWriter(f.Value, false, Encoding.Unicode))
                {
                    foreach (var item in orgl)
                    {
                        writer.WriteLine(item);
                    }
                }
            }
            return mergeList;
        }

        /// <summary>
        /// HTMLのタグを削除
        /// </summary>
        public static string HtmlTagDelete(string html)
        {
            bool tgStart = false;
            System.Text.StringBuilder strB = new System.Text.StringBuilder();
            foreach (char h in html.ToCharArray())
            {
                if (tgStart == true)
                {
                    if (h.Equals('>'))
                    {
                        tgStart = false;
                        //strB.Append(' ');     //tagの終了時に" "追加
                    }
                }
                else
                {
                    if (h.Equals('<'))
                    {
                        tgStart = true;
                    }
                    else
                    {
                        strB.Append(h);
                    }
                }
            }
            strB.Replace("&nbsp;", " ");
            strB.Replace("&lt;", "<");
            strB.Replace("&gt;", ">");
            strB.Replace("&amp;", "&");
            strB.Replace("&#038;", "&");
            strB.Replace("&quot;", "\"");
            return strB.ToString();
        }

        /// <summary>
        ///CSV書き込み
        /// </summary>
        public static void WriteCSV(SG_JSON sgjson, List<CoreTweet.Status> getResponsesUserTimeline)
        {
            ///CSVタイムライン書き込み
            if (getResponsesUserTimeline.Count > 0)
            {
                Console.WriteLine("-----------------------------------------------------------------");
                Console.WriteLine($"CSV書き込み : 書き込み開始");
                var tuid = Directory.GetCurrentDirectory() + "\\" + sgjson.twOutSubDir + "\\" + sgjson.twTimeLineUsersFile;
                using (StreamWriter writer = new StreamWriter(tuid, false, Encoding.Unicode))
                {
                    writer.WriteLine($"UserTimeRun,Id,CreatedAt,CreatedAt.LocalDateTime,User.Id,ScreenName,Language,RetweetedStatus,Source,Text");
                    foreach (var item in getResponsesUserTimeline)
                    {
                        Console.WriteLine($"CSV書き込み : {String.Format("{0, 20}", item.User.ScreenName)} : {item.CreatedAt.LocalDateTime}");
                        var rText = Regex.Replace(item.Text, @"[\t]+", "");
                        rText = Regex.Replace(rText, @"[\n]+", "");
                        rText = Regex.Replace(rText, @"[\r]+", "");
                        writer.WriteLine(
                            $"UserTimeRun," +
                            $"\"{item.Id}\"," +
                            $"\"{item.CreatedAt}\"," +
                            $"\"{item.CreatedAt.LocalDateTime}\"," +
                            $"\"{item.User.Id}\"," +
                            $"\"{item.User.ScreenName}\"," +
                            $"\"{item.Language}\"," +
                            $"\"{item.RetweetedStatus}\"," +
                            $"\"{HtmlTagDelete(item.Source)}\"," +
                            $"\"{rText}" +
                            $"\""
                            );
                    }
                }
                Console.WriteLine($"CSV書き込み : 書き込み終了");
            }
        }

    }
}
