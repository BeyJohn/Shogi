﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Shogi
{
	// 9x9 board
	// 9 pawns
	// bishop -> rook
	// lancer, knight, silver, gold, king, gold, silver, knight, lancer
	public partial class MainWindow : Window
	{

		private static List<Image> BlackPieces = new List<Image>();
		private static List<Image> BlackCapturedPieces = new List<Image>();
		private static List<Image> WhitePieces = new List<Image>();
		private static List<Image> WhiteCapturedPieces = new List<Image>();

		private static Image toMove;
        private static ScaleTransform flip = new ScaleTransform(1, -1);

        public MainWindow()
		{
            InitializeComponent();

            InitializePieces();
		}

		public void InitializePieces()//TODO Place and create picture for rest of the pieces
		{
			BlackKing.Source = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.king.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            BlackKing.RenderTransformOrigin = new Point(.5, .5);
            BlackPawn1.Source = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.Pawn.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            BlackPawn1.RenderTransformOrigin = new Point(.5, .5);

            BlackPieces.Add(BlackKing);
            BlackPieces.Add(BlackPawn1);

			WhiteKing.Source = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.king.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            WhiteKing.RenderTransform = flip;
            WhitePawn1.Source = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.Pawn.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            WhitePawn1.RenderTransform = flip;

            WhitePieces.Add(WhiteKing);
            WhitePieces.Add(WhitePawn1);
		}

		private bool IsValidMove(Image against)
		{
            if (toMove.Name.Contains("Pawn"))
            {
                if (BlackPieces.Contains(toMove))
                {
                    if ((int)against.GetValue(Grid.RowProperty) == (int)toMove.GetValue(Grid.RowProperty) - 1 && (int)toMove.GetValue(Grid.ColumnProperty) == (int)against.GetValue(Grid.ColumnProperty))
                    {
                        return true;
                    }
                }
                else
                {
                    if ((int)against.GetValue(Grid.RowProperty) == (int)toMove.GetValue(Grid.RowProperty) + 1 && (int)toMove.GetValue(Grid.ColumnProperty) == (int)against.GetValue(Grid.ColumnProperty))
                    {
                        return true;
                    }
                }
            }
            return false;
		}

		private bool IsValidMove(Point pos)
		{
			Console.WriteLine(toMove.GetValue(Grid.RowProperty)+":"+ pos.Y+ " " + (int)toMove.GetValue(Grid.ColumnProperty)+":"+pos.X);
            if(toMove.Name.Contains("Pawn"))
            {
                if (BlackPieces.Contains(toMove))
                {
                    if (pos.Y == (int)toMove.GetValue(Grid.RowProperty) - 1 && (int)toMove.GetValue(Grid.ColumnProperty) == pos.X)
                    {
                        return true;
                    }
                }
                else if(WhitePieces.Contains(toMove))
                {
                    if (pos.Y == (int)toMove.GetValue(Grid.RowProperty) + 1 && (int)toMove.GetValue(Grid.ColumnProperty) == pos.X)
                    {
                        return true;
                    }
                }
                else if(BlackCapturedPieces.Contains(toMove))
                {
                    foreach(Image temp in BlackPieces)
                    {
                        if (temp.Name.Contains("Pawn") && (int)temp.GetValue(Grid.ColumnProperty) == pos.X)
                            return false;
                    }
                    return true;
                }
                else if (WhiteCapturedPieces.Contains(toMove))
                {
                    foreach (Image temp in WhitePieces)
                    {
                        if (temp.Name.Contains("Pawn") && (int)temp.GetValue(Grid.ColumnProperty) == pos.X)
                            return false;
                    }
                    return true;
                }
            }
            else if(toMove.Name.Contains("Gold"))
            {
                if(BlackPieces.Contains(toMove))
                {
                    //TODO
                }
                else if(WhitePieces.Contains(toMove))
                {
                    //TODO
                }
                else if(BlackCapturedPieces.Contains(toMove))
                {
                    //TODO
                }
                else if(WhiteCapturedPieces.Contains(toMove))
                {
                    //TODO
                }
            }
            //TODO Rest of the normal pieces and promoted Bishop and Rook
			else if ((int)toMove.GetValue(Grid.RowProperty) != (int)pos.Y || (int)toMove.GetValue(Grid.ColumnProperty) != (int)pos.X)
				return true;
			return false;
		}

		private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Point pos = e.GetPosition(this);
			Console.WriteLine((char)(((int)pos.Y - 75) / 50 + 65) + "-" + (((int)pos.X - 25) / 50 + 1));
			pos.X = ((int)pos.X - 25) / 50;
			pos.Y = ((int)pos.Y - 75) / 50;

			if (toMove != null && IsValidMove(pos))
			{
				toMove.SetValue(Grid.RowProperty, (int)pos.Y);
				toMove.SetValue(Grid.ColumnProperty, (int)pos.X);

				if (!Board.Children.Contains(toMove))
				{
					if (WhiteCapturedGrid.Children.Contains(toMove))
					{
						WhiteCapturedGrid.Children.Remove(toMove);
						WhiteCapturedPieces.Remove(toMove);
						WhitePieces.Add(toMove);
					}
					else
					{
						BlackCapturedGrid.Children.Remove(toMove);
						BlackCapturedPieces.Remove(toMove);
						BlackPieces.Add(toMove);
					}

					Board.Children.Add(toMove);
				}
				toMove = null;
			}
		}

		private void Piece_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (toMove == null)
			{
				if (BlackPieces.Contains((Image)sender))
				{
					Console.WriteLine("BlackPiece: " + ((Image)sender).Name);
				}
				else if (WhitePieces.Contains((Image)sender))
				{
					Console.WriteLine("WhitePiece: " + ((Image)sender).Name);
				}
				else
				{
					Console.WriteLine("Captured Piece");
				}
				toMove = (Image)sender;
			}
			else
			{
				Image tempPiece = (Image)sender;
				if (BlackPieces.Contains(tempPiece) && BlackPieces.Contains(toMove))
				{
					toMove = tempPiece;
				}
				else if(WhitePieces.Contains(tempPiece) && WhitePieces.Contains(toMove))
				{
					toMove = tempPiece;
				}
				else if(WhitePieces.Contains(toMove) && BlackPieces.Contains(tempPiece) && IsValidMove(tempPiece))
				{
					toMove.SetValue(Grid.RowProperty, tempPiece.GetValue(Grid.RowProperty));
					toMove.SetValue(Grid.ColumnProperty, tempPiece.GetValue(Grid.ColumnProperty));

					Board.Children.Remove(tempPiece);
					tempPiece.ClearValue(Grid.RowProperty);
					tempPiece.ClearValue(Grid.ColumnProperty);
					BlackPieces.Remove(tempPiece);
					WhiteCapturedPieces.Add(tempPiece);
					WhiteCapturedGrid.Children.Add(tempPiece);

                    if ((tempPiece).RenderTransform == flip)
                        (tempPiece).RenderTransform = null;
                    else
                        (tempPiece).RenderTransform = flip;

					toMove = null;
				}
				else if (BlackPieces.Contains(toMove) && WhitePieces.Contains(tempPiece) && IsValidMove(tempPiece))
				{
					toMove.SetValue(Grid.RowProperty, tempPiece.GetValue(Grid.RowProperty));
					toMove.SetValue(Grid.ColumnProperty, tempPiece.GetValue(Grid.ColumnProperty));

					Board.Children.Remove(tempPiece);
					tempPiece.ClearValue(Grid.RowProperty);
					tempPiece.ClearValue(Grid.ColumnProperty);
					WhitePieces.Remove(tempPiece);
					BlackCapturedPieces.Add(tempPiece);
					BlackCapturedGrid.Children.Add(tempPiece);

                    if ((tempPiece).RenderTransform == flip)
						(tempPiece).RenderTransform = null;
					else
						(tempPiece).RenderTransform = flip;

					toMove = null;
				}
			}
		}
	}
}
