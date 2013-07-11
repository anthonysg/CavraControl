//
// cavra_usb_test.cs
//
// Author:
//       Steve Beaulac <steve@nca.uwo.ca>
//
// Copyright (c) 2009 The National Centre for Audiology
//
// Licensed under The National Centre for Audiology, Version 1.0
// (the "License"); you may not use this file except in compliance
// with the License.  You may obtain a copy of the License by contacting
//
// The National Centre for Audiology
// http://www.uwo.ca/nca
//
// Unless required by applicable law or agreed to in writing
// software distributed under the License is distributed on an "AS IS"
// BASIS WITHOUT WARRANTIEDS OR CONDITIONS OF ANY KIND. either express
// or implied.  See the License for the specific language governing
// permissions and limitations under the licenses.
//

using System;
using System.Runtime.InteropServices;

using NCA.CavraDriver;

namespace NCA.CavraControl.Test
{
	public class CavraUsbTest
	{

		static void Main(string[] args)
		{
			double db;

			if (args.Length < 1) {
				Console.WriteLine("Missing Parameter.");
				return;
			}
			Console.WriteLine("Set Cavra Attenuator to {0}", args[0]);
			if (double.TryParse(args[0], out db)) {
				Cavra cavra = Cavra.GetInstance();
				cavra.Connect();
				cavra.Attenuator.Left = db;
				cavra.Attenuator.Right = db;
				cavra.Disconnect();
			}
		}
	}
}
