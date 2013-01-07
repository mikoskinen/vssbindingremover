using System;

namespace Engines
{
	public abstract class ConsoleEngineBase
	{
		private string[] m_args;
		protected bool ReadInput = true;
		protected System.IO.TextReader In = null;
		protected System.IO.TextWriter Out = null;
		protected System.IO.TextWriter Error = null;

		public ConsoleEngineBase()
		{
			//by default, read from/write to standard streams
			this.In = System.Console.In;
			this.Out = System.Console.Out;
			this.Error = System.Console.Error;
		}

		public void Main(string[] args)
		{
			this.m_args = args;	

			if(this.ValidateArguments())
			{
				this.PreProcess();
				if(this.ReadInput)
				{
					string currentLine = this.In.ReadLine();
					while(currentLine != null)
					{
						this.ProcessLine(currentLine);
						currentLine = this.In.ReadLine();
					}
				}
				this.PostProcess();
			}
			else
				this.Error.Write("Usage: " + this.Usage());
		}

		public void Main(string[] args, 
			System.IO.TextReader In, 
			System.IO.TextWriter Out, 
			System.IO.TextWriter Error)
		{
			//this version of Main allows alternate streams
			this.In = In;
			this.Out = Out;
			this.Error = Error;
			this.Main(args);
		}

		protected virtual bool ValidateArguments()
		{
			//override this to add custom argument checking
			return true;
		}

		protected virtual string Usage()
		{
			//override this to add custom usage statement 
			return "";
		}

		protected virtual void PreProcess()
		{
			//override this to add custom logic that 
			//executes just before standard in is processed
			return;
		}

		protected virtual void PostProcess()
		{
			//override this to add custom logic that 
			//executes just after standard in is processed
			return;
		}

		protected virtual void ProcessLine(string line)
		{
			//override this to add custom processing 
			//on each line of input
			return;
		}

		protected string[] Arguments
		{
			get {return this.m_args;}
		}
	}
}
