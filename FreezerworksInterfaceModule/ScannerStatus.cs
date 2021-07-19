using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace FreezerworksInterfaceModule {
	class ScannerStatus {
		private bool initialized { get; set; } = false;
		private bool scanning { get; set; } = false;
		private bool finishedScan { get; set; } = false;
		private bool dataReady { get; set; } = false;
		private bool dataSent { get; set; } = false; 
		private bool rack96 { get; set; } = false;
		private bool empty { get; set; } = false;  
		private bool error { get; set; } = false;

		/// <summary>
		/// Overloaded constructor
		/// </summary>
		/// <param name="b">BitArray to initialize scanner status</param>
		public ScannerStatus(BitArray scannerStatus) {
	
			initialized = scannerStatus.Get(0);
			scanning = scannerStatus.Get(1);
			finishedScan = scannerStatus.Get(2);
			dataReady = scannerStatus.Get(3);
			dataSent = scannerStatus.Get(4);
			rack96 = scannerStatus.Get(5);
			empty = scannerStatus.Get(6);
			error = scannerStatus.Get(7);

			Debug.WriteLine("initalized: " + initialized);
			Debug.WriteLine("scanning: " + scanning);
			Debug.WriteLine("finishedScan: " + finishedScan);
			Debug.WriteLine("dataReady: " + dataReady);
			Debug.WriteLine("dataSent: " + dataSent);
			Debug.WriteLine("rack96: " + rack96);
			Debug.WriteLine("empty: " + empty);
			Debug.WriteLine("error: " + error);

		}

		/// <summary>
		/// Return bool indicating if initialized or not
		/// </summary>
		public bool Initialized {
			get { return initialized; }
			set { initialized = value; }
		}

		/// <summary>
		/// Return bool indicating whether scanning or not
		/// </summary>
		public bool Scanning {
			get { return scanning; }
			set { scanning = value; }
		}

		/// <summary>
		/// Return bool indicating whether scan is finished or not
		/// </summary>
		public bool FinishedScan {
			get { return finishedScan; }
			set { finishedScan = value; }
		}

		/// <summary>
		/// Return bool indicating whether data is ready to be sent or not
		/// </summary>
		public bool DataReady {
			get { return dataReady; }
			set { dataReady = value; }
		}

		/// <summary>
		/// Return bool indicating whether data has been sent or not
		/// </summary>
		public bool DataSent {
			get { return dataSent; }
			set { dataSent = value; }
		}

		/// <summary>
		/// Return bool indicating whether rack on scanner is 96 well plate or not
		/// </summary>
		public bool Rack96 {
			get { return rack96; }
			set { rack96 = value; }
		}

		/// <summary>
		/// Return bool indicating whether scanner is empty or not
		/// </summary>
		public bool Empty {
			get { return empty; }
			set { empty = value; }
		}

		/// <summary>
		/// Return bool indicating whether there is error in scanner or not
		/// </summary>
		public bool Error {
			get { return error; }
			set { error = value; }
		}

	}
}
