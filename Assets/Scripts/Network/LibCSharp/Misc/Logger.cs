using System;
using System.IO;
using System.Text;

namespace LibCSharp
{
	public class Logger
	{
		// logs writes per second (RELEASE/DEBUG - minimal difference at all modes < 5%)

		// file + console | console | file (SSD) |    none
		//      2300      |  2400   |  230 000   | 14 000 000

		public enum Level
		{
			Trace,
			Debug,
			Info,
			Warn,
			Success,
			Error,
			No_log
		}

#if UNITY_5 || UNITY_2017 || UNITY_2018 || UNITY_2019 || UNITY_EDITOR
		public static UnityEngine.UI.Text LogText;
#endif

		public static string TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff ";

		public static void Init(string filename, Level console_log_level, Level file_log_level, bool append = false, string timestampFormat = null)
		{
			_console_log_level = console_log_level;
			_file_log_level = file_log_level;

			if (timestampFormat != null)
				TimestampFormat = timestampFormat;

			if (filename != null && filename != "" && file_log_level < Level.No_log)
				_sw = new StreamWriter(filename, append, System.Text.Encoding.Default);
		}

		protected Logger()
		{}

		public static Logger GetCurrentClassLogger()
		{
			return new Logger(); // TODO: set class name
		}

		public bool IsLogged(Level lvl)
		{ return (lvl >= _console_log_level) || (lvl >= _file_log_level); }

		public void Trace(string format, params object[] args)
		{ Log(Level.Trace, format, args); }

		public void Debug(string format, params object[] args)
		{ Log(Level.Debug, format, args); }

		public void Info(string format, params object[] args)
		{ Log(Level.Info, format, args); }

		public void Warn(string format, params object[] args)
		{ Log(Level.Warn, format, args); }

		public void Error(string format, params object[] args)
		{ Log(Level.Error, format, args); }

		public void Success(string format, params object[] args)
		{ Log(Level.Success, format, args); }

		void Log(Level lvl, string format, params object[] args)
		{
			return;
			
			if (!IsLogged(lvl))	return;

			StringBuilder sb = new StringBuilder(256);
			sb.Append(DateTime.Now.ToString(TimestampFormat));
			sb.AppendFormat(format, args);
			string log_str = sb.ToString();

			lock (mutex) // thread lock
			{
				if (lvl >= _console_log_level)
				{
#if UNITY_5 || UNITY_2017 || UNITY_2018 || UNITY_2019 || UNITY_EDITOR

					if (LogText != null)
						LogText.text += log_str + Environment.NewLine;

					if (lvl == Level.Error || lvl == Level.Warn)
						UnityEngine.Debug.LogWarning(log_str); // with (!)
					else
						UnityEngine.Debug.Log(log_str);
#else
					ConsoleColor lvl_clr = _color[(int)lvl];
					ConsoleColor old_clr = Console.ForegroundColor;
					if (old_clr != lvl_clr)
						Console.ForegroundColor = lvl_clr;

					Console.WriteLine(log_str);

					if (old_clr != lvl_clr)
						Console.ForegroundColor = old_clr;
#endif
				}

				if (lvl >= _file_log_level && _sw != null)
				{
					_sw.WriteLine(log_str);
					_sw.Flush();
				}
			}
		}

		static Level _console_log_level;
		static Level _file_log_level;

		static StreamWriter _sw;

#if UNITY_5 || UNITY_2017 || UNITY_2018 || UNITY_2019 || UNITY_EDITOR
#else
		static readonly ConsoleColor[] _color = new ConsoleColor[Enum.GetNames(typeof(Level)).Length];

		static Logger()
		{
			if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
				_color[(int)Level.Trace] = ConsoleColor.DarkGray;
			else
				_color[(int)Level.Trace] = ConsoleColor.DarkBlue;

			_color[(int)Level.Debug] = ConsoleColor.Gray;
			_color[(int)Level.Info] = ConsoleColor.White;
			_color[(int)Level.Warn] = ConsoleColor.Yellow;
			_color[(int)Level.Error] = ConsoleColor.Red;
			_color[(int)Level.Success] = ConsoleColor.Green;
		}
#endif
		static readonly object mutex = new object();
	}
}