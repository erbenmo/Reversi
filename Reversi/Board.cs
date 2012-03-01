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

		public List<List<PIECE>> board;
		
		public List<int> empty_neighbors_list; // location idx of all qualified pieces

		/* data from http://www.site-constructor.com/othello/Present/BoardLocationValue.html */
		int [][] initial_weight = {new int []{50, -1, 5, 2, 2, 5, -1, 50},
								   new int [] {-1, -10, 1, 1, 1, 1, -10, -1},
								   new int [] {5, 1, 1, 1, 1, 1, 1, 5},
								   new int [] {2, 1, 1, 0, 0, 1, 1, 2},
								   new int [] {2, 1, 1, 0, 0, 1, 1, 2},
								   new int [] {5, 1, 1, 1, 1, 1, 1, 5},
								   new int [] {-1, -10, 1, 1, 1, 1, -10, -1},
								   new int [] {50, -1, 5, 2, 2, 5, -1, 50}};

		public Board()
		{
			Debug.Assert(SIZE % 2 == 0);

			board = new List<List<PIECE>>();
			empty_neighbors_list = new List<int>();

			for (int h = 0; h < SIZE; h++)
			{	
				List<PIECE> row_pieces = new List<PIECE>();

				for (int w = 0; w < SIZE; w++)
				{
					row_pieces.Add(PIECE.EMPTY);
				}
				board.Add(row_pieces);
			}

			int mid_1 = SIZE / 2 - 1, mid_2 = SIZE / 2;
			place_piece(mid_1, mid_2, PIECE.BLACK, MARK.Mark);
			place_piece(mid_2, mid_1, PIECE.BLACK, MARK.Mark);
			place_piece(mid_1, mid_1, PIECE.WHITE, MARK.Mark);
			place_piece(mid_2, mid_2, PIECE.WHITE, MARK.Mark);

		}

		public void print()
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

		public int getStrength(PIECE caller)
		{
			int strength = 0;
			for (int h = 0; h < SIZE; h++)
			{
				for (int w = 0; w < SIZE; w++)
				{
					if (board[h][w] == caller)
						strength += initial_weight[h][w];
				}
			}

			return strength;
		}

		public List<int> find_legal_candidates(PIECE caller)
		{
			List<int> legal_cand = new List<int>();

			for (int i = 0; i < empty_neighbors_list.Count; i++)
			{
				int h = empty_neighbors_list[i] / SIZE, w = empty_neighbors_list[i] % SIZE;

				if(isLegal(h, w, caller))
					legal_cand.Add(empty_neighbors_list[i]);
			}

			return legal_cand;
		}

		







		public int place_piece(int h, int w, PIECE caller, MARK need_mark, WEIGHT weight = WEIGHT.Default)
		{
			if (!on_board(h, w) || board[h][w] != PIECE.EMPTY) return 0;
			
			int gain_sum = 0;
			if (need_mark == MARK.Mark) board[h][w] = caller;

			gain_sum += move_up(h, w, caller, need_mark, weight);
			gain_sum += move_down(h, w, caller, need_mark, weight);
			gain_sum += move_left(h, w, caller, need_mark, weight);
			gain_sum += move_right(h, w, caller, need_mark, weight);
			gain_sum += move_upper_left(h, w, caller, need_mark, weight);
			gain_sum += move_lower_right(h, w, caller, need_mark, weight);
			gain_sum += move_lower_left(h, w, caller, need_mark, weight);
			gain_sum += move_upper_right(h, w, caller, need_mark, weight);

			if (need_mark == MARK.Mark)
				maintain_empty_neighbors_list(h, w);
			
			return gain_sum;
		}

		public void maintain_empty_neighbors_list(int h, int w)
		{
			int idx = h * SIZE + w;
			empty_neighbors_list.Remove(idx);
			for (int dh = -1; dh <= 1; dh++)
			{
				for (int dw = -1; dw <= 1; dw++)
				{
					int new_idx = (h + dh) * SIZE + w + dw;
					if (dh == 0 && dw == 0) continue;
					
					if (empty_neighbors_list.IndexOf(new_idx) == -1 && 
						on_board(h+dh, w+dw) &&
						board[h+dh][w+dw] == PIECE.EMPTY)
						empty_neighbors_list.Add(new_idx);
				}
			}
		}



		public bool isLegal(int h, int w, PIECE caller)
		{
			if (on_board(h, w) == false) return false;
			if (move_up(h, w, caller, MARK.Don_t_mark) != 0) return true;
			if (move_down(h, w, caller, MARK.Don_t_mark) != 0) return true;
			if (move_left(h, w, caller, MARK.Don_t_mark) != 0) return true;
			if (move_right(h, w, caller, MARK.Don_t_mark) != 0) return true;
			if (move_upper_left(h, w, caller, MARK.Don_t_mark) != 0) return true;
			if (move_lower_right(h, w, caller, MARK.Don_t_mark) != 0) return true;
			if (move_upper_right(h, w, caller, MARK.Don_t_mark) != 0) return true;
			if (move_lower_left(h, w, caller, MARK.Don_t_mark) != 0) return true;
			return false;
		}

		public bool on_board(int h, int w)
		{
			return h < SIZE && h >= 0 && w < SIZE && w >= 0;
		}

		public int move_up(int h, int w, PIECE caller, MARK need_mark, WEIGHT weight = WEIGHT.Default)
		{
			PIECE enemy = (caller == PIECE.BLACK) ? PIECE.WHITE : PIECE.BLACK;
			h = h - 1;
			while (on_board(h, w) && board[h][w] == enemy) h = h - 1;
			if (!on_board(h, w) || board[h][w] == PIECE.EMPTY) return 0;

			h = h + 1;
			int gain = 0;
			while (board[h][w] == enemy)
			{
				if (need_mark == MARK.Mark) board[h][w] = caller;
				h = h + 1;
				gain += getGain(h, w, weight);
			}
			return gain;
		}

		public int move_down(int h, int w, PIECE caller, MARK need_mark, WEIGHT weight = WEIGHT.Default)
		{
			PIECE enemy = (caller == PIECE.BLACK) ? PIECE.WHITE : PIECE.BLACK;
			h = h + 1;
			while (on_board(h, w) && board[h][w] == enemy) h = h + 1;
			if (!on_board(h, w) || board[h][w] == PIECE.EMPTY) return 0;

			h = h - 1;
			int gain = 0;
			while (board[h][w] == enemy)
			{
				if (need_mark == MARK.Mark) board[h][w] = caller;
				h = h - 1;
				gain += getGain(h, w, weight);
			}
			return gain;
		}

		public int move_left(int h, int w, PIECE caller, MARK need_mark, WEIGHT weight = WEIGHT.Default)
		{
			PIECE enemy = (caller == PIECE.BLACK) ? PIECE.WHITE : PIECE.BLACK;
			w = w - 1;
			while (on_board(h, w) && board[h][w] == enemy) w = w - 1;
			if (!on_board(h, w) || board[h][w] == PIECE.EMPTY) return 0;

			w = w + 1;
			int gain = 0;
			while (board[h][w] == enemy)
			{
				if (need_mark == MARK.Mark) board[h][w] = caller;
				w = w + 1;
				gain += getGain(h, w, weight);
			}
			return gain;
		}

		public int move_right(int h, int w, PIECE caller, MARK need_mark, WEIGHT weight = WEIGHT.Default)
		{
			PIECE enemy = (caller == PIECE.BLACK) ? PIECE.WHITE : PIECE.BLACK;
			w = w + 1;
			while (on_board(h, w) && board[h][w] == enemy) w = w + 1;
			if (!on_board(h, w) || board[h][w] == PIECE.EMPTY) return 0;

			w = w - 1;
			int gain = 0;
			while (board[h][w] == enemy)
			{
				if (need_mark == MARK.Mark) board[h][w] = caller;
				w = w - 1;
				gain += getGain(h, w, weight);
			}
			return gain;
		}

		public int move_upper_left(int h, int w, PIECE caller, MARK need_mark, WEIGHT weight = WEIGHT.Default)
		{
			PIECE enemy = (caller == PIECE.BLACK) ? PIECE.WHITE : PIECE.BLACK;
			w = w - 1; h = h - 1;
			while (on_board(h, w) && board[h][w] == enemy) { w = w - 1; h = h - 1; }
			if (!on_board(h, w) || board[h][w] == PIECE.EMPTY) return 0;

			w = w + 1; h = h + 1;
			int gain = 0;
			while (board[h][w] == enemy)
			{
				if (need_mark == MARK.Mark) board[h][w] = caller;
				w = w + 1; h = h + 1;
				gain += getGain(h, w, weight);
			}
			return gain;
		}

		public int move_lower_right(int h, int w, PIECE caller, MARK need_mark, WEIGHT weight = WEIGHT.Default)
		{
			PIECE enemy = (caller == PIECE.BLACK) ? PIECE.WHITE : PIECE.BLACK;
			w = w + 1; h = h + 1;
			while (on_board(h, w) && board[h][w] == enemy) { w = w + 1; h = h + 1; }
			if (!on_board(h, w) || board[h][w] == PIECE.EMPTY) return 0;

			w = w - 1; h = h - 1;
			int gain = 0;
			while (board[h][w] == enemy)
			{
				if (need_mark == MARK.Mark) board[h][w] = caller;
				w = w - 1; h = h - 1;
				gain += getGain(h, w, weight);
			}
			return gain;
		}

		public int move_lower_left(int h, int w, PIECE caller, MARK need_mark, WEIGHT weight = WEIGHT.Default)
		{
			PIECE enemy = (caller == PIECE.BLACK) ? PIECE.WHITE : PIECE.BLACK;
			w = w - 1; h = h + 1;
			while (on_board(h, w) && board[h][w] == enemy) { w = w - 1; h = h + 1; }
			if (!on_board(h, w) || board[h][w] == PIECE.EMPTY) return 0;

			w = w + 1; h = h - 1;
			int gain = 0;
			while (board[h][w] == enemy)
			{
				if (need_mark == MARK.Mark) board[h][w] = caller;
				w = w + 1; h = h - 1;
				gain += getGain(h, w, weight);
			}
			return gain;
		}

		public int move_upper_right(int h, int w, PIECE caller, MARK need_mark, WEIGHT weight = WEIGHT.Default)
		{
			PIECE enemy = (caller == PIECE.BLACK) ? PIECE.WHITE : PIECE.BLACK;
			w = w + 1; h = h - 1;
			while (on_board(h, w) && board[h][w] == enemy) { w = w + 1; h = h - 1; }
			if (!on_board(h, w) || board[h][w] == PIECE.EMPTY) return 0;

			w = w - 1; h = h + 1;
			int gain = 0;
			while (board[h][w] == enemy)
			{
				if (need_mark == MARK.Mark) board[h][w] = caller;
				w = w - 1; h = h + 1;
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