using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Web.Razor;
using System.IO;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Diagnostics;

namespace HtmlGen
{
    public partial class Form1 : Form
    {
        private TemplateBase _currentTemplate;
        private RazorTemplateEngine _engine;

        public TemplateBase CurrentTemplate
        {
            get
            {
                return _currentTemplate;
            }
            set
            {
                _currentTemplate = value;
                UpdateTemplate();
            }
        }

        public Form1()
        {
            InitializeComponent();
            _engine = SetupRazorEngine();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CurrentTemplate = null;

            // Generate code for the template
            GeneratorResults razorResult = null;
            using (TextReader rdr = new StringReader(templateTextBox.Text))
            {
                razorResult = _engine.GenerateCode(rdr);
            }

            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            
            // Generate the code and put it in the text box:
            using (StringWriter sw = new StringWriter())
            {
                codeProvider.GenerateCodeFromCompileUnit(razorResult.GeneratedCode, sw, new CodeGeneratorOptions());
                generatedCodeTextBox.Text = sw.GetStringBuilder().ToString();
            }

            // Compile the generated code into an assembly
            string outputAssemblyName = String.Format("Temp_{0}.dll", Guid.NewGuid().ToString("N"));
            CompilerResults results = codeProvider.CompileAssemblyFromDom(
                new CompilerParameters(new string[] {
                    typeof(Form1).Assembly.CodeBase.Replace("file:///", "").Replace("/", "\\")
                }, outputAssemblyName),
                razorResult.GeneratedCode);

            if (results.Errors.HasErrors)
            {
                CompilerError err = results.Errors
                                           .OfType<CompilerError>()
                                           .Where(ce => !ce.IsWarning)
                                           .First();
                MessageBox.Show(String.Format("Error Compiling Template: ({0}, {1}) {2}",
                                              err.Line, err.Column, err.ErrorText));
            }
            else
            {
                // Load the assembly
                Assembly asm = Assembly.LoadFrom(outputAssemblyName);
                if (asm == null)
                {
                    MessageBox.Show("Error loading template assembly");
                }
                else
                {
                    // Get the template type
                    Type typ = asm.GetType("RazorOutput.Template");
                    if (typ == null)
                    {
                        MessageBox.Show("Could not find type RazorOutput.Template in assembly {0}", asm.FullName);
                    }
                    else
                    {
                        TemplateBase newTemplate = Activator.CreateInstance(typ) as TemplateBase;
                        if (newTemplate == null)
                        {
                            MessageBox.Show("Could not construct RazorOutput.Template or it does not inherit from TemplateBase");
                        }
                        else
                        {
                            CurrentTemplate = newTemplate;
                        }
                    }
                }
            }
        }

        private void UpdateTemplate()
        {
            propertyGrid1.SelectedObject = CurrentTemplate;
        }

        private RazorTemplateEngine SetupRazorEngine()
        {
            // Set up the hosting environment

            // a. Use the C# language (you could detect this based on the file extension if you want to)
            RazorEngineHost host = new RazorEngineHost(new CSharpRazorCodeLanguage());

            // b. Set the base class
            host.DefaultBaseClass = typeof(TemplateBase).FullName;

            // c. Set the output namespace and type name
            host.DefaultNamespace = "RazorOutput";
            host.DefaultClassName = "Template";

            // d. Add default imports
            host.NamespaceImports.Add("System");

            // Create the template engine using this host
            return new RazorTemplateEngine(host);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (CurrentTemplate == null)
            {
                MessageBox.Show("No Template Loaded!");
            }
            else
            {
                CurrentTemplate.Execute();
                outputTextBox.Text = CurrentTemplate.Buffer.ToString();
                CurrentTemplate.Buffer.Clear();
            }
        }
    }
}
