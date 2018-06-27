using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShimZip {
   static class Program {
      /// <summary>
      /// 해당 응용 프로그램의 주 진입점입니다.
      /// </summary>
      [STAThread]
      static void Main(string[] args) {
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);
         if (args.Length == 0) {
            Application.Run(new FormMain());
         } else if (args[0] == "-o") {
            if (args.Length >= 2)
               Application.Run(new FormMain(args[1]));
         } else if (args[0] == "-x" || args[0] == "-a") {
            if (args.Length >= 2)
               CommandLine.Run(args);
         }
      }
   }
}
