//
// SmPlayer.cs
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
using System.IO;
using System.Media;

namespace CavraControl
{
	public class SmPlayer : IPlayer
	{
		SoundPlayer player;

		public SmPlayer()
		{
			player = new SoundPlayer();
		}

		public virtual void Load(string wavFile)
		{
			var stream = File.OpenRead(wavFile);
			player.Stream = stream;
			player.Load ();
		}

		public virtual void Play()
		{
			player.Play();
		}

		public virtual void Stop()
		{
			player.Stop();
		}
	}
}
