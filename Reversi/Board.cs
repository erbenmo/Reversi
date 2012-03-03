using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Reversi
{
	class Board
	{
		public enum PIECE
		{
			BLACK,
			WHITE,
			EMPTY
		}

		public enum MARK
		{
			Mark,
			Don_t_mark,
		}

		public enum WEIGHT
		{
			Static,
			Default,
			Dynamic
		}

		public const int SIZE = 8;

		int [][] initial_weight = {new int []{50, -1, 5, 2, 2, 5, -1, 50},
								   new int [] {-1, -10, 1, 1, 1, 1, -10, -1},
								   new int [] {5, 1, 1, 1, 1, 1, 1, 5},
								   new int [] {2, 1, 1, 0, 0, 1, 1, 2},
								   new int [] {2, 1, 1, 0, 0, 1, 1, 2},
								   new int [] {5, 1, 1, 1, 1, 1, 1, 5},
								   new int [] {-1, -10, 1, 1, 1, 1, -10, -1},
								   new int [] {50, -1, 5, 2, 2, 5, -1, 50}};

		public Board(List<List<Board.PIECE>> initial_board, List<int> initial_empty_neighbor_list)
		{
			Debug.Assert(SIZE % 2 == 0);

			for (int h = 0; h < SIZE; h++)
			{	
				List<PIECE> row_pieces = new List<PIECE>();

				for (int w = 0; w < SIZE; w++)
				{
					row_pieces.Add(PIECE.EMPTY);
				}
				initial_board.Add(row_pieces);
			}

			int mid_1 = SIZE / 2 - 1, mid_2 = SIZE / 2;
			place_piece(initial_board, initial_empty_neighbor_list, mid_1, mid_2, PIECE.BLACK, MARK.Mark);
			place_piece(initial_board, initial_empty_neighbor_list, mid_2, mid_1, PIECE.BLACK, MARK.Mark);
			place_piece(initial_board, initial_empty_neighbor_list, mid_1, mid_1, PIECE.WHITE, MARK.Mark);
			place_piece(initial_board, initial_empty_neighbor_list, mid_2, mid_2, PIECE.WHITE, MARK.Mark);
		}

		public void print(List<List<Board.PIECE>> board)
		{
			Console.Write("R ");
			for (int i = 0; i < Board.SIZE; i++)
			{
				Console.Write(i);
				Console.Write(" ");
			}
			Console.WriteLine();

			for (int h = 0; h < SIZE; h++)
			{
				System.Console.Write(h);
				System.Console.Write(" ");
				for (int w = 0; w < SIZE; w++)
				{
					char cur = ' ';
					if (board[h][w] == PIECE.BLACK) cur = '@';
					else if (board[h][w] == PIECE.WHITE) cur = '-';
					else { }
					System.Console.Write(cur);
					System.Console.Write(" ");
				}
				System.Console.WriteLine("");
			}
			System.Console.WriteLine();
			System.Console.WriteLine();
		}

		public double getStrengthRatio(List<List<Board.PIECE>> board)
		{
			int strength_black = 1, strength_white = 1;
			for (int h = 0; h < SIZE; h++)
			{
				for (int w = 0; w < SIZE; w++)
				{
					if (board[h][w] == PIECE.BLACK)
						strength_black += initial_weight[h][w];
					else if (board[h][w] == PIECE.WHITE )
						strength_white += initial_weight[h][w];
				}
			}

			return 1.0 * strength_black / strength_white;
		}

		public List<int> find_legal_candidates(List<List<Board.PIECE>> board, List<int> empty_neighbor_list, PIECE caller)
		{
			List<int> legal_cand = new List<int>();

			for (int i = 0; i < empty_neighbor_list.Count; i++)
			{
				int h = empty_neighbor_list[i] / SIZE, w = empty_neighbor_list[i] % SIZE;

				if(isLegal(board, h, w, caller))
					legal_cand.Add(empty_neighbor_list[i]);
			}

			return legal_cand;
		}

		
		public int place_piece(List<List<Board.PIECE>> board, List<int> empty_neighbor_list, 
			int h, int w, PIECE caller, MARK need_mark, WEIGHT weight = WEIGHT.Default)
		{
			if (!on_board(h, w) || board[h][w] != PIECE.EMPTY) return 0;
			
			int gain_sum = 0;
			if (need_mark == MARK.Mark) board[h][w] = caller;

			for (int dh = -1; dh <= 1; dh++)
			{
				for (int dw = -1; dw <= 1; dw++)
				{
					if(dh == 0 && dw == 0) continue;

					gain_sum += move(board, h, w, dh, dw, caller, need_mark, weight);
				}
			}
			
			if (need_mark == MARK.Mark)
				maintain_empty_neighbors_list(board, empty_neighbor_list, h, w);
			
			return gain_sum;
		}

		public void maintain_empty_neighbors_list(List<List<Board.PIECE>> board, List<int> empty_neighbor_list, int h, int w)
		{
			int idx = h * SIZE + w;
			empty_neighbor_list.Remove(idx);
			for (int dh = -1; dh <= 1; dh++)
			{
				for (int dw = -1; dw <= 1; dw++)
				{
					int new_idx = (h + dh) * SIZE + w + dw;
					if (dh == 0 && dw == 0) continue;
					
					if (empty_neighbor_list.IndexOf(new_idx) == -1 && 
						on_board(h+dh, w+dw) &&
						board[h+dh][w+dw] == PIECE.EMPTY)
						empty_neighbor_list.Add(new_idx);
				}
			}
		}


		public bool isLegal(List<List<Board.PIECE>> board, int h, int w, PIECE caller)
		{
			if (on_board(h, w) == false) return false;

			for (int dh = -1; dh <= 1; dh++)
			{
				for (int dw = -1; dw <= 1; dw++)
				{
					if (dh == 0 && dw == 0) continue;
					if (move(board, h, w, dh, dw, caller, MARK.Don_t_mark) != 0) return true;
				}
			}

			return false;
		}

		public bool on_board(int h, int w)
		{
			return h < SIZE && h >= 0 && w < SIZE && w >= 0;
		}


		public int move(List<List<Board.PIECE>> board, int h, int w, int dh, int dw, 
			PIECE caller, MARK need_mark, WEIGHT weight = WEIGHT.Default)
		{
			PIECE enemy = (caller == PIECE.BLACK) ? PIECE.WHITE : PIECE.BLACK;
			w = w + dw; h = h + dh;
			while (on_board(h, w) && board[h][w] == enemy) { w = w + dw; h = h + dh; }
			if (!on_board(h, w) || board[h][w] == PIECE.EMPTY) return 0;

			w = w - dw; h = h - dh;
			int gain = 0;
			while (board[h][w] == enemy)
			{
				if (need_mark == MARK.Mark) board[h][w] = caller;
				w = w - dw; h = h - dh;
				gain += getGain(h, w, weight);
			}
			return gain;
		}

		public int getGain(int h, int w, WEIGHT weight)
		{
			if (weight == WEIGHT.Default) return 1;
			if (weight == WEIGHT.Static) return initial_weight[h][w];
			if (weight == WEIGHT.Dynamic) return initial_weight[h][w];

			Debug.Assert(false);
			return -1;
		}
	}
}