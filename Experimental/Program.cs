using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using IronPython.Hosting;

namespace WindowsFormsApplication1
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			var engine = Python.CreateEngine();
			//new Random().Next()
			var scope = engine.ExecuteFile("script/battle.py");

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form2());
		}
	}
}
