//
//  Author:
//    steve ${AuthorEmail}
//
//  Copyright (c) 2013, steve
//
//  All rights reserved.
//
//  Licensed under The National Centre for Audiology, Version 1.0
//  (the "License"); you may not use this file except in compliance
//  with the License.  You may obtain a copy of the License by contacting
//
//  The National Centre for Audiology
//  http://www.uwo.ca/nca/
//
//  Unless required by applicable law or agreed to in writing
//  software distributed under the License is distributed on an "AS IS"
//  BASIS WITHOUT WARRANTIEDS OR CONDITIONS OF ANY KIND. either express
//  or implied.  See the License for the specific language governing
//  permissions and limitations under the licenses.
//
using System;

namespace NCA.CavraDriver
{
	public interface IAttenuator
	{
		double Left { get; set; }
		double Right { get; set; }

		void Mute();
	}
}

