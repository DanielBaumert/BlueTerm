using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BlueTerm.UI
{
    public class Card : Control
    {
        public event Action<Card, int, int> Moved;

        #region Title
        private string _title;
        private Font _titleFont;
        private Padding _titlePadding;
        private Color _titleBackColor;
        private Color _titleColor;

        private Size _titleBgSize;
        private Size _titleSize;
        private Point _titlePoint;
        private Rectangle _titleBgRect;
        #region Properties
        public string Title {
            get => _title;
            set {
                if (_title != value)
                {
                    _title = value;
                    UpdateTitleSetting();
                    UpdateSizeSetting();
                    Invalidate();
                }
            }
        }
        public Font TitleFont {
            get => _titleFont;
            set {
                if (_titleFont != value)
                {
                    _titleFont = value;
                    UpdateTitleSetting();
                    UpdateSizeSetting();
                    Invalidate();
                }
            }
        }
        public Padding TitlePadding {
            get => _titlePadding;
            set {
                if (_titlePadding != value)
                {
                    _titlePadding = value;
                    UpdateTitleSetting();
                    UpdateSizeSetting();
                    Invalidate();
                }
            }
        }
        public Color TitleBack {
            get => _titleBackColor;
            set {
                if (_titleBackColor != value)
                {
                    _titleBackColor = value;
                    Invalidate();
                }
            }
        }
        public Color TitleColor {
            get => _titleColor;
            set {
                if (_titleColor != value)
                {
                    _titleColor = value;
                    Invalidate();
                }
            }
        }
        #endregion
        #endregion

        private Font _portFont;
        private Size _portSize;
        private (string inWord, string outWord) _portMaxName;

        private Rectangle _portRect = Rectangle.Empty;

        private bool Dragging = false;
        private Point DragStart = Point.Empty;
        public Direction Direction { get; set; }

        public Font PortFont {
            get => _portFont;
            set {
                if (_portFont != value)
                {
                    _portFont = value;
                    _portSize = TextRenderer.MeasureText("Example", _portFont);


                    UpdateSizeSetting();
                    Invalidate();
                }
            }
        }

        private PortItemCollection _inPorts;
        private PortItemCollection _outPorts;

        public PortItemCollection InPorts {
            get => _inPorts;
            set {
                if (value != null)
                {
                    if (_inPorts != value)
                    {
                        _inPorts = value.ToList();
                        if (_inPorts.Any())
                        {
                            _portMaxName.inWord = _inPorts.OrderByDescending(x => x.Name.Length).First().Name;
                            UpdateSizeSetting();
                            Invalidate();
                        }
                    }
                }
            }
        }
        public PortItemCollection OutPorts {
            get => _outPorts;
            set {
                if (value != null)
                {
                    if (_outPorts != value)
                    {
                        _outPorts = value.ToList();
                        if (_outPorts.Any())
                        {
                            _portMaxName.outWord = _outPorts.OrderByDescending(x => x.Name.Length).First().Name;
                            UpdateSizeSetting();
                            Invalidate();
                        }
                    }
                }
            }
        }
        public Card()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.CacheText, true);
            InPorts = Array.Empty<PortItem>();
            OutPorts = Array.Empty<PortItem>();
            TitleBack = Color.FromArgb(192, 0, 0);
            TitleColor = Color.FromArgb(64, 64, 64);
            BackColor = Color.FromArgb(32, 32, 32);
            Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            TitlePadding = new Padding(8);
        }
        private void UpdateTitleSetting()
        {
            _titleSize = TextRenderer.MeasureText(_title, _titleFont);
            _titleBgSize = new Size
            {
                Width = _titlePadding.Horizontal + _titleSize.Width,
                Height = _titlePadding.Vertical + _titleSize.Height
            };

            //TODO Items height

            _titlePoint = new Point
            {
                X = (Width - _titleSize.Width) / 2,
                Y = (_titleBgSize.Height - _titleSize.Height) / 2
            };
            _titleBgRect = new Rectangle
            {
                Location = Point.Empty,
                Width = Width,
                Height = _titleBgSize.Height
            };

            _portRect.Y = _titleBgRect.Height;
        }


        private void UpdateSizeSetting()
        {

            _portRect.Height = _inPorts.Count > _outPorts.Count
                           ? _inPorts.Count * (_portSize.Height + 4)
                           : _outPorts.Count * (_portSize.Height + 4);
            _portRect.Width = TextRenderer.MeasureText(_portMaxName.inWord, _portFont).Width
                + 16
                + TextRenderer.MeasureText(_portMaxName.outWord, _portFont).Width;

            if (_titleBgSize.Width > Width || _portRect.Width > Width)
            {
                if (_titleBgSize.Width > Width)
                {
                    Width = _titleBgSize.Width;
                }
                else
                {
                    Width = _portRect.Width;
                }
            }

            if (_portRect.Height + _titleBgRect.Height > Height)
            {
                Height = _portRect.Height + _titleBgRect.Height;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;

            using (SolidBrush titleBgbrush = new SolidBrush(_titleBackColor))
            {
                graphics.FillRectangle(titleBgbrush, _titleBgRect);
            }

            TextRenderer.DrawText(graphics, _title, _titleFont, _titlePoint, _titleColor);

            if (_inPorts.Any())
            {
                int startY = _portRect.Y + 8;
                foreach (var item in _inPorts)
                {
                    TextRenderer.DrawText(graphics, item.Name, Font, new Point { X = 12, Y = startY }, ForeColor);
                    startY += _portSize.Height + 8;
                }
            }

            if (_outPorts.Any())
            {
                int startY = _portRect.Y + 8;
                foreach (var item in _outPorts)
                {
                    Size currentSize = TextRenderer.MeasureText(item.Name, _portFont);
                    TextRenderer.DrawText(graphics, item.Name, Font, new Point { X = Width - 12 - currentSize.Width, Y = startY }, ForeColor);
                    startY += _portSize.Height + 8;
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            UpdateTitleSetting();
            UpdateSizeSetting();
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Dragging = true;
                DragStart = new Point(e.X, e.Y);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (Dragging)
                {
                    if (Direction != Direction.Vertical)
                        Left = Math.Max(0, e.X + Left - DragStart.X);
                    if (Direction != Direction.Horizontal)
                        Top = Math.Max(0, e.Y + Top - DragStart.Y);

                    Moved?.Invoke(this, Left, Top);
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            Dragging = false;
        }
    }
}
