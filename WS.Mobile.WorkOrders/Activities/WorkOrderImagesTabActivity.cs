using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Widget;
using WS.Mobile.WorkOrders.Backend;
using Android.Util;
using Android.Content.PM;

namespace WS.Mobile.WorkOrders.Activities
{
	[Activity]
	public class WorkOrderImagesTabActivity : AbstractTabItemActivity
	{
		private const int CameraRequest = 1888;
		private TableLayout _imagesTable;
		private Java.IO.File _lastPictureFile;
		private string _type;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate(bundle);
			_type =Intent.GetStringExtra ("type");
			SetContentView(Resource.Layout.workOrderImagesTab);

			var addButton = FindViewById<Button>(Resource.Id.workOrderTakePhotoButton);
			addButton.Click += OnTakePhotoClick;

			_imagesTable = FindViewById<TableLayout>(Resource.Id.workOrderImagesTable);
			LoadChanges();
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			outState.PutString ("lastPictureFile", _lastPictureFile == null ? null : _lastPictureFile.Path);
			base.OnSaveInstanceState (outState);
		}

		protected override void OnRestoreInstanceState (Bundle savedInstanceState)
		{
			_lastPictureFile = null;

			if (savedInstanceState.GetString ("lastPictureFile") != null)
				_lastPictureFile = new Java.IO.File (savedInstanceState.GetString ("lastPictureFile"));

			base.OnRestoreInstanceState (savedInstanceState);
		}

		private void ScaleImage (string imagePath, int longSide)
		{
			// get the source image's dimensions
			var options = new BitmapFactory.Options();
			options.InJustDecodeBounds = true;
			BitmapFactory.DecodeFile(imagePath, options);

			var srcWidth = options.OutWidth;
			var srcHeight = options.OutHeight;
			var srcLongSide = Math.Max(srcWidth, srcHeight);
			
			// Calculate the correct inSampleSize/scale value. This helps reduce memory use. It should be a power of 2
			// from: http://stackoverflow.com/questions/477572/android-strange-out-of-memory-issue/823966#823966
			var inSampleSize = 1;
			while (srcLongSide / 2 > longSide) {
				srcLongSide /= 2;
				srcWidth /= 2;
				srcHeight /= 2;
				inSampleSize *= 2;
			}

			var desiredScale = (float)longSide / (float)srcLongSide;

			// Decode with inSampleSize
			options.InJustDecodeBounds = false;
			options.InDither = false;
			options.InSampleSize = inSampleSize;
			options.InScaled = false;
			options.InPreferredConfig = Bitmap.Config.Argb8888;

			var sampledSrcBitmap = BitmapFactory.DecodeFile(imagePath, options);

			// Resize
			var matrix = new Matrix();
			matrix.PostScale(desiredScale, desiredScale);

			var scaledBitmap = Bitmap.CreateBitmap(sampledSrcBitmap, 0, 0, sampledSrcBitmap.Width, sampledSrcBitmap.Height, matrix, true);

			sampledSrcBitmap.Dispose();
			sampledSrcBitmap = null;

			if (File.Exists(imagePath))
				File.Delete(imagePath);

			// Save
			var fileStream = System.IO.File.Create(imagePath);
			scaledBitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, fileStream);

			scaledBitmap.Dispose();
			scaledBitmap = null;
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			if (requestCode == CameraRequest && resultCode == Result.Ok) {
				var lastPictureUri = Android.Net.Uri.FromFile(_lastPictureFile);
				var id = _lastPictureFile.Name.Replace(".jpg", "");

				ScaleImage(_lastPictureFile.Path, 1024);

				// make it available in the gallery
				var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
				mediaScanIntent.SetData(lastPictureUri);
				SendBroadcast(mediaScanIntent);

				var imageObj = new JsonObject();
				imageObj["ID"] = id;
				imageObj["ImagePath"] = _lastPictureFile.Path;
				imageObj["Location"] = "E";
				if (_type == Global.FieldOffRentType) {
					imageObj ["Damaged"] = false;
				}
				AddObjectToCollection(Global.FieldImages, imageObj);
				AddRow(imageObj);
			}
		}

		private void OnTakePhotoClick (object sender, EventArgs e)
		{
			var cameraIntent = new Intent(MediaStore.ActionImageCapture);

			var availableActivities = this.PackageManager.QueryIntentActivities(cameraIntent, Android.Content.PM.PackageInfoFlags.MatchDefaultOnly);

			if (availableActivities != null && availableActivities.Count > 0) {
				var unit = GetActiveWorkOrder();
				var unitNumber = (string)unit["UnitNumber"];

				var dir = new Java.IO.File(
							Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures),
							unitNumber);

				if (!dir.Exists()) {
					var x = dir.Mkdirs();
				}

				_lastPictureFile = new Java.IO.File(dir, String.Format("{0}-{1}.jpg", unitNumber, Guid.NewGuid()));

				cameraIntent.PutExtra(
							MediaStore.ExtraOutput, 
								Android.Net.Uri.FromFile(_lastPictureFile));

				StartActivityForResult(cameraIntent, CameraRequest);
			}
		}
		
		private void AddRow (JsonObject imageObj)
		{
			var row = new TableRow(this);
			var id = (string)imageObj["ID"];

			var imagePath = (string)imageObj["ImagePath"];
			var sampleSize = 4;
			
			if (LandscapeOrientation) {
				sampleSize = 4;
			} else {
				sampleSize = 8;
			}

			using (var image = BitmapFactory.DecodeFile(imagePath, new BitmapFactory.Options {
					InSampleSize = sampleSize 
				})) {
				var imageView = new ImageView(this);
				imageView.SetImageBitmap(image);
				
				row.AddView(imageView);
			}

			var location = (string)imageObj["Location"];
			var interiorRadio = new RadioButton(this);
			interiorRadio.Text = GetString(Resource.String.workOrder_interior);
			interiorRadio.Checked = location == "I";

			var exteriorRadio = new RadioButton(this);
			exteriorRadio.Text = GetString(Resource.String.workOrder_exterior);
			exteriorRadio.Checked = location == "E";

			interiorRadio.CheckedChange += (sender, e) => {
				if (e.IsChecked) {
					exteriorRadio.Checked = false;
					UpdateCollectionProperty(Global.FieldImages, id, "Location", "I");
				}
			};
			exteriorRadio.CheckedChange += (sender, e) => {
				if (e.IsChecked) {
					interiorRadio.Checked = false;
					UpdateCollectionProperty(Global.FieldImages, id, "Location", "E");
				}
			};

			var locationRadioGroup = new RadioGroup(this);
			locationRadioGroup.AddView(interiorRadio);
			locationRadioGroup.AddView(exteriorRadio);
			row.AddView(locationRadioGroup);
			if (_type == Global.FieldOffRentType) {
				var damaged = (bool)imageObj ["Damaged"];
				var damagedCheck = new CheckBox (this);
				damagedCheck.Text = GetString (Resource.String.workOrder_damage);
				damagedCheck.Checked = damaged;
				damagedCheck.CheckedChange += (sender, e) => {
					var value = e.IsChecked;
					UpdateCollectionProperty (Global.FieldImages, id, "Damaged", value);
				};
			
				row.AddView (damagedCheck);
			} else {
				row.AddView (new TextView (this));
			}
			row.AddView (new TextView (this));

			var removeButton = new Button(this);
			removeButton.Text = GetString(Resource.String.workOrder_remove);
			removeButton.Click += delegate {
				RemoveRow(id, row);
			};
			row.AddView(removeButton);
			
			_imagesTable.AddView(row);
		}
		
		private void RemoveRow (string id, TableRow row)
		{
			RemoveObjectFromCollection(Global.FieldImages, id);
			
			RunOnUiThread(delegate {
				_imagesTable.RemoveView(row);
			});
		}

		protected override void OnResume ()
		{
			//always supress load changes, this gets called onCreate
			SupressLoadChanges = true;
			base.OnResume ();
		}

		protected override void LoadChanges ()
		{
			var unit = GetActiveWorkOrder();
			// remove all rows except the header
			while(_imagesTable.ChildCount > 1)
				_imagesTable.RemoveViewAt(1);

			var images = (List<object>)unit[Global.FieldImages];
			if (images != null) {
				foreach (var image in images)
					AddRow((JsonObject)image);
			}
		}
	}
}
