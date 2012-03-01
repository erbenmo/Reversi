using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reversi
{
	class Reversi
	{
		public Board b = new Board();

		public static void Main()
		{
			Reversi game = new Reversi();
			game.play();
		}

		public void play()
		{
			int safe = 0;
			Board.PIECE turn = Board.PIECE.BLACK;
			while ((safe+=1) < 100)
			{
				b.print();

				if (turn == Board.PIECE.BLACK)
				{
					List<int> legal_cand = b.find_legal_candidates(turn);
					if (legal_cand.Count != 0)
					{
						int h = -1, w = -1;
						System.Console.WriteLine("@");
						getInput(ref h, ref w);

						b.place_piece(h, w, turn, Board.MARK.Mark);
					}
					turn = Board.PIECE.WHITE;
				}
				else if (turn == Board.PIECE.WHITE)
				{
					List<int> legal_cand = b.find_legal_candidates(turn);
					if (legal_cand.Count != 0)
					{
						int index = b.Random_AI(turn, legal_cand);
						int h = index / Board.SIZE, w = index % Board.SIZE;
						
						System.Console.WriteLine("-");
						Console.WriteLine(h + " " + w);
						Console.WriteLine();
						Console.WriteLine();
						
						b.place_piece(h, w, turn, Board.MARK.Mark);
					}
					turn = Board.PIECE.BLACK;
				}
			}
		}

		public void getInput(ref int h, ref int w)
		{
			string[] tokens = Console.ReadLine().Split();
			h = int.Parse(tokens[0]);
			w = int.Parse(tokens[1]);
			System.Console.WriteLine();
			System.Console.WriteLine();
		}
	}
}
