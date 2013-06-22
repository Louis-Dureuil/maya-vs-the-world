using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace LuxEngine
{
	public class Interpreter
	{
		public Interpreter (string name, 
		                    bool forceRecompilation,
		                    bool forbidCompilation)
		{
			this.name = name;
			string assetName = Path.GetFileNameWithoutExtension (name) + ".dll";
			bool compilation = forceRecompilation || !File.Exists (assetName);
			if (compilation && forbidCompilation) {
				// compilation is mandatory... and forbidden
				throw new Exception ("A compilation is mandatory, but the current settings forbids compilation. Sorry!");
			}

			if (compilation) {
				if (!Directory.Exists (name)) {
					// could not find script directory
					throw new DirectoryNotFoundException (String.Format ("A compilation is mandatory for the assembly {0}, but the matching directory {1} cannot be found.",
					                                                   assetName,
					                                                   name));
				}
				// list all files
				string[] scripts = Directory.GetFiles (name, "*.cs");
				// do the compilation
				CodeDomProvider provider = new CSharpCodeProvider ();
				CompilerParameters parameters = new CompilerParameters ();
				parameters.CompilerOptions = "/target:library";
				parameters.GenerateExecutable = false;
				parameters.OutputAssembly = String.Format (assetName);
				parameters.IncludeDebugInformation = false;
                parameters.ReferencedAssemblies.Add("LuxEngine.dll");
                parameters.ReferencedAssemblies.Add(Environment.GetEnvironmentVariable("XNAGSv4") + @"References\Windows\x86\Microsoft.Xna.Framework.dll");
                parameters.ReferencedAssemblies.Add(Environment.GetEnvironmentVariable("XNAGSv4") + @"References\Windows\x86\Microsoft.Xna.Framework.Game.dll");
				CompilerResults results = 
					provider.CompileAssemblyFromFile (parameters,
					                                  scripts);
				// check for errors
				if (results.Errors.HasErrors) {
					throw new Exception (String.Format ("The compilation of {0} failed.",
					                                    assetName));
				}
				this.assembly = results.CompiledAssembly;
			} else {
				// load the assembly
				assembly = Assembly.LoadFrom (assetName);
			}

		}

		public string Name {
			get { return name; }
		}

		public Interpretable GetInterpretable (string interpretableName, Scene parent)
		{
			Type type = assembly.GetType (interpretableName);
			if (type == null) {
				throw new Exception (String.Format ("Cannot find type {0} in interpreter {1}.",
				                                    interpretableName,
				                                    this.name));
			}
            object[] args = new object[1] {parent};
			return Activator.CreateInstance (type, args) as Interpretable;
		}

		private string name;
		private Assembly assembly;
	}
}
