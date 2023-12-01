using PicturesEditor.Core;
using System;

namespace PicturesEditor.ModalDialogWindows
{
    internal sealed class AsciiOptionsDialogVM : ObservableObject
    {
		public event Action<(double widthOffset, int maxWidth, bool highQuality, bool fromBrightToDim)> OnApply;

		private double _widthOffset = 2d;
		public double WidthOffset
		{
			get => _widthOffset;
			set
			{
				_widthOffset = value;
				OnPropertyChanged();
			}
		}

		private int _maxWidth = 200;
		public int MaxWidth
		{
			get => _maxWidth;
			set
			{
				_maxWidth = value;
				OnPropertyChanged();
			}
		}

		private bool _highQUality = false;
		public bool HighQuality
		{
			get => _highQUality;
			set
			{
				_highQUality = value;
				OnPropertyChanged();
			}
		}

		private bool _fromBrightToDim = true;
		public bool FromBrightToDim
		{
			get => _fromBrightToDim;
			set
			{
				_fromBrightToDim = value;
				OnPropertyChanged();
			}
		}

		public RelayCommand ApplyCommand { get; private set; }

        public AsciiOptionsDialogVM()
        {
            ApplyCommand = new RelayCommand(_ => OnApply?.Invoke((WidthOffset, MaxWidth, HighQuality, FromBrightToDim)));
        }
    }
}
