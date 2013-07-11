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
using System.Collections.Generic;
using System.Threading;

using SjB.Hid;

namespace NCA.CavraDriver
{
	public class Cavra
	{
		const int VID = 0x0d8c;
		const int PID = 0x000c;
		const int MUTE_LEVEL = 0x4F;

		const int LEFT_CHANNEL = 0x0000;
		const int RIGHT_CHANNEL = 0x0001;

		static readonly byte[] START_MESSAGE = {0x3, 0x2, 0x0};
		static readonly byte[] TERMINATE_MESSAGE = {0x0, 0x2, 0x3};

		public const double MAX_DB_LEVEL = 76.0;
		public const double MIN_DB_LEVEL = 0.0;

		HidDevice hiddev;

		public static Cavra driver = null;
		public static Cavra GetInstance ()
		{
			if (null == driver) {
				driver = new Cavra ();
			}
			return driver;
		}

		class CavraAttenuator : IAttenuator
		{

			public double Left {
				get { return device.current_channel_setting[Cavra.LEFT_CHANNEL]; }
				set {
					PushAttenuatorSetting(Cavra.LEFT_CHANNEL, value);
				}
			}

			public double Right {
				get { return device.current_channel_setting[Cavra.RIGHT_CHANNEL]; }
				set {
					PushAttenuatorSetting(Cavra.RIGHT_CHANNEL, value);
				}
			}

			Cavra device;
			public CavraAttenuator(Cavra cavra)
			{
				device = cavra;
			}

			void PushAttenuatorSetting (int channel, double level)
			{
				device.SendAttenuatorLevelToDevice(channel, level);
			}

			public void Mute()
			{
				PushAttenuatorSetting(Cavra.LEFT_CHANNEL, Cavra.MAX_DB_LEVEL);
				PushAttenuatorSetting(Cavra.RIGHT_CHANNEL, Cavra.MAX_DB_LEVEL);
			}
		}

		public IAttenuator Attenuator { get; private set; }

		Dictionary<int, double> current_channel_setting = new Dictionary<int, double>();
		Dictionary<int, double> requested_channel_setting = new Dictionary<int, double>();

		AutoResetEvent signal;
		Thread updater;
		bool thread_quit = false;

		public Cavra ()
		{
			Attenuator = new CavraAttenuator(this);
			InitializedChannelLevel(Cavra.LEFT_CHANNEL);
			InitializedChannelLevel(Cavra.RIGHT_CHANNEL);
			signal = new AutoResetEvent(false);
		}

		void InitializedChannelLevel(int channel)
		{
			current_channel_setting[channel] = requested_channel_setting[channel] = Cavra.MAX_DB_LEVEL;
		}

		public void Connect ()
		{
			hiddev = new HidDevice(VID, PID);
			hiddev.Open();
			updater = new Thread(new ThreadStart(AttenuatorUpdaterProcess));
			updater.IsBackground = true;
			updater.Start();
		}

		public void Disconnect ()
		{
			lock(updater) {
				thread_quit = true;
			}
			signal.Set();
			updater.Join();
			updater = null;
			hiddev.Close();
		}


		void AttenuatorUpdaterProcess()
		{
			while(HasLevelChanged(Cavra.LEFT_CHANNEL) || HasLevelChanged(Cavra.RIGHT_CHANNEL) || signal.WaitOne()) {
				lock(updater) {
					if (thread_quit)
						break;
				}

				UpdateChannel(Cavra.LEFT_CHANNEL);
				UpdateChannel(Cavra.RIGHT_CHANNEL);
				signal.Reset();
			}
		}

		void UpdateChannel(int channel)
		{
			if (HasLevelChanged(channel)) {
				double level;
				lock (requested_channel_setting) {
					level = requested_channel_setting[channel];
				}
				UpdateAttenuatorLevel(channel, level);
			}
		}

		bool HasLevelChanged(int channel)
		{
			lock(requested_channel_setting) {
				return requested_channel_setting[channel] != current_channel_setting[channel];
			}
		}

		void SendAttenuatorLevelToDevice(int channel, double db)
		{
			lock(requested_channel_setting) {
				requested_channel_setting[channel] = db;
			}
			signal.Set();
		}

		int UpdateAttenuatorLevel(int channel, double db)
		{
			int count = 0;
			int reg = DBToHexValue(db);
			count += SendMessage(START_MESSAGE);
			count += SendChannelId(channel);
			count += SendRawLevelValue(reg);
			count += SendMessage(TERMINATE_MESSAGE);
			return count;
		}

		int DBToHexValue(double db)
		{
			int reg = MUTE_LEVEL;

			if (MIN_DB_LEVEL > db) {
				reg = 0;
			} else if (16.0 > db) {
				reg = (int)Math.Round(db * 2.0);
			} else if (48.0 > db) {
				reg = (0x20 + (int)Math.Round(db - 16.0));
			} else if (MAX_DB_LEVEL >= db) {
				reg = (0x41 + (int)Math.Round((db - 50.0) / 2.0));
			}
			return reg;
		}

		int SendRawLevelValue(int level)
		{
			int count = 0;
			for (int i = 7; i >= 0; i--) {
				byte b = (byte)(0x0f & (level >> i));
				count += SendData(b);
			}
			return count;
		}

		int SendData(byte b)
		{
			int count = 0;
			count += SendByte((byte)(0x01 & b));
			count += SendByte((byte)(0x02 | b));
			count += SendByte((byte)(0x01 & b));
			return count;
		}

		int SendByte(byte b)
		{
			byte[] sequence = {0x0, 0x0, 0xf, 0x0};
			sequence[1] = b;
			return hiddev.Write(sequence);
		}

		int SendMessage(byte[] mesg)
		{
			int nbytes = 0;
			foreach (byte b in mesg)
				nbytes += SendByte(b);
			return nbytes;
		}

		int SendChannelId(int id)
		{
			int count = 0;
			count += SendData((byte)(0x000f & (id >> 8)));
			count += SendData((byte)(0x000f & id));
			return count;
		}
	}
}

