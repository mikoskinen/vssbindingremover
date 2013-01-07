using System;
using System.Text.RegularExpressions;
using VSSBindingRemover.Library;

namespace Engines
{
	public class VSSBindingRemoverEngine : ConsoleEngineBase
	{
		protected override void PreProcess()
		{
			for (int i = 0; i < this.Arguments.Length; i++)
				this.ProcessLine(this.Arguments[i]);
		}

		protected override void ProcessLine(string line)
		{
			try
			{
				VSSBindingRemover.Library.VSSBindingRemover vssRemover = new VSSBindingRemover.Library.VSSBindingRemover(line);
				vssRemover.RemoveBindings();

				this.Out.WriteLine("Processing completed without errors.");
			}
			catch (Exception ex)
			{
				this.Error.WriteLine(ex.ToString());
			}
		}

		protected override bool ValidateArguments()
		{
			if (this.Arguments.Length > 0)
				this.ReadInput = false; //cancel reading input if directory names are given as args
			return true;
		}

	}
}
