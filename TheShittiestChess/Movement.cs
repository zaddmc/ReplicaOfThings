﻿using System.Diagnostics;

namespace TheShittiestChess
{
    internal class Movement
    {
        public static List<Button> removeLater = new List<Button>();
        private static bool IsPostitionOccupied(Position position)
        {
            return MainPage.piecePostions.ContainsKey(MainPage.PositionToString(position));
        } // IsPostitionOccupied
        private static bool IsPieceTakeable(Position position, bool IsThePlayerWhite)
        {
            if (IsPostitionOccupied(position))
                if (MainPage.piecePostions[MainPage.PositionToString(position)].isWhite != IsThePlayerWhite)
                    return true;
            return false;
        } // IsPieceTakeable
        private static void TakePiece(Position enemyPosition, ChessPiece pieceTryingToTake)
        {
            if (IsPieceTakeable(enemyPosition, pieceTryingToTake.isWhite))
            {
                // "killing" the enemy piece
                MainPage.piecePostions[MainPage.PositionToString(enemyPosition)].isAlive = false;
                MainPage.Board.Remove(MainPage.imageButtons[MainPage.piecePostions[MainPage.PositionToString(enemyPosition)].index]);

                MainPage.piecePostions.Remove(MainPage.PositionToString(enemyPosition));


                MovePiece(pieceTryingToTake.position, enemyPosition);
            }
            else
            {
                Debug.WriteLine("The Chosen Piece is not Takeable");
            }
        } // TakePiece
        private static void MovePiece(Position oldPostition, Position newPostition)
        {
            if (!IsPostitionOccupied(newPostition))
            {
                var thePiece = MainPage.piecePostions[MainPage.PositionToString(oldPostition)];
                MainPage.chessPieceche[thePiece.index].position = newPostition;

                // removes the old allocated postion in the dictionary and allocates the new position
                MainPage.piecePostions.Remove(MainPage.PositionToString(oldPostition));
                MainPage.piecePostions.Add(MainPage.PositionToString(newPostition), MainPage.chessPieceche[thePiece.index]);

                // moves the given piece
                Grid.SetColumn(MainPage.imageButtons[thePiece.index], newPostition.x);
                Grid.SetRow(MainPage.imageButtons[thePiece.index], newPostition.y);

                // the round counter goes up
                MainPage.chessPieceche[thePiece.index].lastMoveRound = MainPage.currentRound;
                MainPage.currentRound++;

                // to change the box
                MainPage.TurnBox(true);
            }
            else
            {
                Debug.WriteLine("the chosen destination is occupied");
            }
        } // MovePiece
        private static void CheckSurroundings(ChessPiece thePiece, Position positionToCheck)
        {
            if (positionToCheck.x >= 0 && positionToCheck.x <= 7 && positionToCheck.y >= 0 && positionToCheck.y <= 7)
            {
                char ccolor = '?';
                Color color = new Color();
                if (IsPostitionOccupied(positionToCheck))
                {
                    if (IsPieceTakeable(positionToCheck, thePiece.isWhite)) { color = Colors.Red; ccolor = 'R'; }
                    else { color = Colors.Yellow; ccolor = 'Y'; }
                }
                else { color = Colors.Green; ccolor = 'G'; }

                Color backColor;
                if (positionToCheck.x % 2 == 0)
                {
                    if (positionToCheck.y % 2 == 0)
                        backColor = Colors.AliceBlue;
                    else
                        backColor = Colors.Navy;
                }
                else
                {
                    if (positionToCheck.y % 2 == 1)
                        backColor = Colors.AliceBlue;
                    else
                        backColor = Colors.Navy;
                }


                string passOn = string.Format(MainPage.PositionToString(thePiece.position) + "|" + MainPage.PositionToString(positionToCheck));
                Button button = new Button
                {
                    //IsVisible = false,
                    Opacity = 0,
                    ZIndex = 30,
                    ClassId = passOn,

                };
                Button border = new Button
                {
                    BorderColor = color,
                    BorderWidth = 5,
                    BackgroundColor = backColor,
                    ZIndex = 15,
                };
                MainPage.Board.Children.Add(border);
                Grid.SetColumn(border, positionToCheck.x);
                Grid.SetRow(border, positionToCheck.y);
                removeLater.Add(border);

                if (ccolor != 'Y')
                {
                    MainPage.Board.Children.Add(button);
                    Grid.SetColumn(button, positionToCheck.x);
                    Grid.SetRow(button, positionToCheck.y);
                    removeLater.Add(button);
                }

                switch (ccolor)
                {
                    case 'R':
                        button.Clicked += TakePiece_Clicked;
                        break;

                    case 'Y':
                        button.Clicked += SelectNewPiece_Clicked;
                        break;

                    case 'G':
                        button.Clicked += MovePiece_Clicked;
                        break;

                    default:
                        Debug.WriteLine("I dont know how you manged to press a button and yet not press anything");
                        break;
                }
            }
        } // CheckSurroundings
        private static void TakePiece_Clicked(object sender, EventArgs e)
        {
            // setup of information
            var button = sender as Button;
            string[] passedOn = button.ClassId.Split('|');
            ChessPiece thePiece = MainPage.piecePostions[passedOn[0]];
            Position newPosition = new Position(passedOn[1][2] - 48, passedOn[1][7] - 48);

            if (passedOn.Length == 3) // en passant moment
            {
                Position enPassant = new Position(passedOn[2][2] - 48, passedOn[2][7] - 48);
                TakePiece(newPosition, thePiece, enPassant);
            }
            else // the standerd to take
                TakePiece(newPosition, thePiece);


            ClearRemoveLater();

            Debug.WriteLine("The piece Should have moved to a new position");
        } // TakePiece_Clicked
        private static void SelectNewPiece_Clicked(object sender, EventArgs e)
        {
            // this button/method should not be possible to activate
            // setup of information
            var button = sender as Button;
            string[] passedOn = button.ClassId.Split('|');
            ChessPiece thePiece = MainPage.piecePostions[passedOn[0]];
            Position newPosition = new Position(passedOn[1][2] - 48, passedOn[1][7] - 48);

            Debug.WriteLine("how did you activate this button?");
        }
        private static void MovePiece_Clicked(object sender, EventArgs e)
        {
            // setup of information
            var button = sender as Button;
            string[] passedOn = button.ClassId.Split('|');
            ChessPiece thePiece = MainPage.piecePostions[passedOn[0]];
            Position newPosition = new Position(passedOn[1][2] - 48, passedOn[1][7] - 48);

            if (passedOn.Length == 4) // casteling moment
                MovePiece(thePiece.position, new Position(passedOn[1][2] - 48, passedOn[1][7] - 48), new Position(passedOn[2][2] - 48, passedOn[2][7] - 48), new Position(passedOn[3][2] - 48, passedOn[3][7] - 48)); // do this
            else // the standard route
                MovePiece(thePiece.position, newPosition);


            ClearRemoveLater();

            Debug.WriteLine("The piece Should have moved to a new position");
        } // MovePiece_Clicked
        public static void ClearRemoveLater()
        {
            while (removeLater.Count > 0)
            {
                MainPage.Board.Remove(removeLater.First());
                removeLater.RemoveAt(0);
            }
        } // ClearRemoveLater
        public static List<Position> KingPositions(ChessPiece thePiece)
        {
            List<Position> kingPositions = new List<Position>
            {
            new Position(thePiece.position.x - 1, thePiece.position.y + 1),
            new Position(thePiece.position.x, thePiece.position.y + 1),
            new Position(thePiece.position.x + 1, thePiece.position.y + 1),
            new Position(thePiece.position.x + 1, thePiece.position.y),
            new Position(thePiece.position.x + 1, thePiece.position.y - 1),
            new Position(thePiece.position.x, thePiece.position.y - 1),
            new Position(thePiece.position.x - 1, thePiece.position.y - 1),
            new Position(thePiece.position.x - 1, thePiece.position.y),
            };
            return kingPositions;
        }
        public static void KingMover(ChessPiece thePiece)
        {

            if (MainPage.piecePostions[thePiece.position.ToString()].lastMoveRound == 0)
            {
                // long casteling
                Position castleLong = new(thePiece.position.x - 4, thePiece.position.y);
                if (MainPage.piecePostions.ContainsKey(castleLong.ToString()) &&
                    !IsPostitionOccupied(new Position(thePiece.position.x - 1, thePiece.position.y)) &&
                    !IsPostitionOccupied(new Position(thePiece.position.x - 2, thePiece.position.y)) &&
                    !IsPostitionOccupied(new Position(thePiece.position.x - 3, thePiece.position.y)))
                {
                    if (MainPage.piecePostions[castleLong.ToString()].pieceType == ChessPiece.PieceTypes.rook &&
                        MainPage.piecePostions[castleLong.ToString()].lastMoveRound == 0)
                    {
                        CheckSurroundings(thePiece, new Position(thePiece.position.x - 2, thePiece.position.y), castleLong, new Position(thePiece.position.x - 1, thePiece.position.y));
                    }
                }
                // short castling
                Position castleShort = new(thePiece.position.x + 3, thePiece.position.y);
                if (MainPage.piecePostions.ContainsKey(castleShort.ToString()) &&
                    !IsPostitionOccupied(new Position(thePiece.position.x + 1, thePiece.position.y)) &&
                    !IsPostitionOccupied(new Position(thePiece.position.x + 2, thePiece.position.y)))
                {
                    if (MainPage.piecePostions[castleShort.ToString()].pieceType == ChessPiece.PieceTypes.rook &&
                        MainPage.piecePostions[castleShort.ToString()].lastMoveRound == 0)
                    {
                        CheckSurroundings(thePiece, new Position(thePiece.position.x + 2, thePiece.position.y), castleShort, new Position(thePiece.position.x + 1, thePiece.position.y));
                    }
                }
            }
            List<Position> kingPositions = KingPositions(thePiece);
            for (int i = 0; i < kingPositions.Count; i++)
            {
                if (CheckTheKingStep(thePiece, kingPositions[i]))
                {
                    CheckSurroundings(thePiece, kingPositions[i]);
                }
            }
        } // KingMover
        public static bool CanTheKingMove(ChessPiece thePiece)
        {
            List<Position> kingPos = KingPositions(thePiece);

            for (int i = 0; i < kingPos.Count; i++)
                if (!CheckTheKingStep(thePiece, kingPos[i]))
                    return false;

            return true;
        }
        public static bool CheckTheKingStep(ChessPiece thePiece, Position newPosition)
        {
            /// <summary>
            /// Used to cycle thru kingpostions to check if it is a legal move
            /// </summary>

            bool[] bools = new bool[8];
            for (int j = 0; j < bools.Length; j++)
                bools[j] = true;

            for (int i = 1; i < 8; i++)
            {
                //diagnol
                List<Position> checksDia = new List<Position>();
                if (bools[0])
                {
                    Position temp = new Position(newPosition.x + i, newPosition.y + i);
                    checksDia.Add(temp);
                    if (IsPostitionOccupied(temp))
                        bools[0] = false;
                }
                if (bools[1])
                {
                    Position temp = new Position(newPosition.x - i, newPosition.y + i);
                    checksDia.Add(temp);
                    if (IsPostitionOccupied(temp))
                        bools[1] = false;
                }
                if (bools[2])
                {
                    Position temp = new Position(newPosition.x - i, newPosition.y - i);
                    checksDia.Add(temp);
                    if (IsPostitionOccupied(temp))
                        bools[2] = false;
                }
                if (bools[3])
                {
                    Position temp = new Position(newPosition.x + i, newPosition.y - i);
                    checksDia.Add(temp);
                    if (IsPostitionOccupied(temp))
                        bools[3] = false;
                }
                for (int j = 0; j < checksDia.Count; j++)
                    if (IsPostitionOccupied(checksDia[j]))
                        if (MainPage.piecePostions[checksDia[j].ToString()].pieceType == ChessPiece.PieceTypes.queen &&
                            MainPage.piecePostions[checksDia[j].ToString()].isWhite != thePiece.isWhite ||
                            MainPage.piecePostions[checksDia[j].ToString()].pieceType == ChessPiece.PieceTypes.bishop &&
                            MainPage.piecePostions[checksDia[j].ToString()].isWhite != thePiece.isWhite)
                            return false;

                //linear
                List<Position> checksLin = new List<Position>();
                if (bools[4])
                {
                    Position temp = new Position(newPosition.x + i, newPosition.y);
                    checksLin.Add(temp);
                    if (IsPostitionOccupied(temp))
                        bools[4] = false;
                }
                if (bools[5])
                {
                    Position temp = new Position(newPosition.x - i, newPosition.y);
                    checksLin.Add(temp);
                    if (IsPostitionOccupied(temp))
                        bools[5] = false;
                }
                if (bools[6])
                {
                    Position temp = new Position(newPosition.x, newPosition.y + i);
                    checksLin.Add(temp);
                    if (IsPostitionOccupied(temp))
                        bools[6] = false;
                }
                if (bools[7])
                {
                    Position temp = new Position(newPosition.x, newPosition.y - i);
                    checksLin.Add(temp);
                    if (IsPostitionOccupied(temp))
                        bools[7] = false;
                }
                for (int j = 0; j < checksLin.Count; j++)
                    if (IsPostitionOccupied(checksLin[j]))
                        if (MainPage.piecePostions[checksLin[j].ToString()].pieceType == ChessPiece.PieceTypes.queen &&
                            MainPage.piecePostions[checksLin[j].ToString()].isWhite != thePiece.isWhite ||
                            MainPage.piecePostions[checksLin[j].ToString()].pieceType == ChessPiece.PieceTypes.rook &&
                            MainPage.piecePostions[checksLin[j].ToString()].isWhite != thePiece.isWhite)
                            return false;
            }
            // knight
            Position[] knightPositions =
            {
                new Position(newPosition.x + 2, newPosition.y + 1),
                new Position(newPosition.x + 2, newPosition.y - 1),
                new Position(newPosition.x - 2, newPosition.y + 1),
                new Position(newPosition.x - 2, newPosition.y - 1),
                new Position(newPosition.x + 1, newPosition.y + 2),
                new Position(newPosition.x - 1, newPosition.y + 2),
                new Position(newPosition.x + 1, newPosition.y - 2),
                new Position(newPosition.x - 1, newPosition.y - 2),
            };
            for (int i = 0; i < knightPositions.Length; i++)
                if (IsPostitionOccupied(knightPositions[i]))
                    if (MainPage.piecePostions[knightPositions[i].ToString()].pieceType == ChessPiece.PieceTypes.knight &&
                        MainPage.piecePostions[knightPositions[i].ToString()].isWhite != thePiece.isWhite)
                        return false;

            // pawn
            int polarrization = 1;
            if (!thePiece.isWhite)
                polarrization = -1;

            // attack position to the right
            Position[] pawnPos =
            {
                new Position(newPosition.x + 1 * polarrization, newPosition.y + 1 * polarrization),
                new Position(newPosition.x - 1 * polarrization, newPosition.y + 1 * polarrization),
            };

            for (int i = 0; i < pawnPos.Length; i++)
                if (IsPostitionOccupied(pawnPos[i]))
                    if (MainPage.piecePostions[pawnPos[i].ToString()].pieceType == ChessPiece.PieceTypes.pawn &&
                        MainPage.piecePostions[pawnPos[i].ToString()].isWhite != thePiece.isWhite)
                        return false;

            return true;
        }
        private static void CheckSurroundings(ChessPiece thePiece, Position newKingPos, Position oldRookPos, Position newRookPos)
        {
            string passOn = string.Format(MainPage.PositionToString(thePiece.position) + "|" + MainPage.PositionToString(newKingPos) + "|" + MainPage.PositionToString(oldRookPos) + "|" + MainPage.PositionToString(newRookPos));
            Button button = new Button
            {
                Opacity = 0,
                ZIndex = 30,
                ClassId = passOn,

            };
            Button border = new Button
            {
                BorderColor = Colors.Green,
                BorderWidth = 5,
                ZIndex = 15,
            };
            MainPage.Board.Children.Add(border);
            Grid.SetColumn(border, newKingPos.x);
            Grid.SetRow(border, newKingPos.y);
            removeLater.Add(border);

            MainPage.Board.Children.Add(button);
            Grid.SetColumn(button, newKingPos.x);
            Grid.SetRow(button, newKingPos.y);
            removeLater.Add(button);

            button.Clicked += MovePiece_Clicked;

        } // CheckSurroundings for the king
        private static void MovePiece(Position oldKingPos, Position newKingPos, Position oldRookPos, Position newRookPos)
        {
            var theKing = MainPage.piecePostions[MainPage.PositionToString(oldKingPos)];
            MainPage.chessPieceche[theKing.index].position = newKingPos;

            var theRook = MainPage.piecePostions[MainPage.PositionToString(oldRookPos)];
            MainPage.chessPieceche[theRook.index].position = newRookPos;

            // removes the old allocated postion in the dictionary and allocates the new position for the king
            MainPage.piecePostions.Remove(MainPage.PositionToString(oldKingPos));
            MainPage.piecePostions.Add(MainPage.PositionToString(newKingPos), MainPage.chessPieceche[theKing.index]);

            // removes the old allocated postion in the dictionary and allocates the new position for the rook
            MainPage.piecePostions.Remove(MainPage.PositionToString(oldRookPos));
            MainPage.piecePostions.Add(MainPage.PositionToString(newRookPos), MainPage.chessPieceche[theRook.index]);

            // moves the king
            Grid.SetColumn(MainPage.imageButtons[theKing.index], newKingPos.x);
            Grid.SetRow(MainPage.imageButtons[theKing.index], newKingPos.y);

            // moves the rook
            Grid.SetColumn(MainPage.imageButtons[theRook.index], newRookPos.x);
            Grid.SetRow(MainPage.imageButtons[theRook.index], newRookPos.y);

            // the round counter goes up
            MainPage.chessPieceche[theKing.index].lastMoveRound = MainPage.currentRound;
            MainPage.chessPieceche[theRook.index].lastMoveRound = MainPage.currentRound;
            MainPage.currentRound++;

            // to change the box
            MainPage.TurnBox(true);
        } // MovePiece for the kings casteling
        public static void QueenMover(ChessPiece thePiece)
        {
            List<Position> queenPositions = new List<Position>();
            bool[] bools = new bool[8];
            for (int i = 0; i < bools.Length; i++)
                bools[i] = true;

            for (int i = 1; i < 8; i++)
            {
                //diagnol
                if (bools[0])
                {
                    Position temp = new Position(thePiece.position.x + i, thePiece.position.y + i);
                    queenPositions.Add(temp);
                    if (IsPostitionOccupied(temp))
                        bools[0] = false;
                }
                if (bools[1])
                {
                    Position temp = new Position(thePiece.position.x - i, thePiece.position.y + i);
                    queenPositions.Add(temp);
                    if (IsPostitionOccupied(temp))
                        bools[1] = false;
                }
                if (bools[2])
                {
                    Position temp = new Position(thePiece.position.x - i, thePiece.position.y - i);
                    queenPositions.Add(temp);
                    if (IsPostitionOccupied(temp))
                        bools[2] = false;
                }
                if (bools[3])
                {
                    Position temp = new Position(thePiece.position.x + i, thePiece.position.y - i);
                    queenPositions.Add(temp);
                    if (IsPostitionOccupied(temp))
                        bools[3] = false;
                }

                //linear
                if (bools[4])
                {
                    Position temp = new Position(thePiece.position.x + i, thePiece.position.y);
                    queenPositions.Add(temp);
                    if (IsPostitionOccupied(temp))
                        bools[4] = false;
                }
                if (bools[5])
                {
                    Position temp = new Position(thePiece.position.x - i, thePiece.position.y);
                    queenPositions.Add(temp);
                    if (IsPostitionOccupied(temp))
                        bools[5] = false;
                }
                if (bools[6])
                {
                    Position temp = new Position(thePiece.position.x, thePiece.position.y + i);
                    queenPositions.Add(temp);
                    if (IsPostitionOccupied(temp))
                        bools[6] = false;
                }
                if (bools[7])
                {
                    Position temp = new Position(thePiece.position.x, thePiece.position.y - i);
                    queenPositions.Add(temp);
                    if (IsPostitionOccupied(temp))
                        bools[7] = false;
                }
            }
            for (int i = 0; i < queenPositions.Count; i++)
            {
                CheckSurroundings(thePiece, queenPositions[i]);
            }


        } // QueenMover
        public static void BishopMover(ChessPiece thePiece)
        {
            List<Position> bishopPositions = new List<Position>();
            bool[] bools = new bool[4];
            for (int i = 0; i < bools.Length; i++)
                bools[i] = true;

            for (int i = 1; i < 8; i++)
            {
                if (bools[0])
                {
                    Position temp = new Position(thePiece.position.x + i, thePiece.position.y + i);
                    bishopPositions.Add(temp);
                    if (IsPostitionOccupied(temp))
                        bools[0] = false;
                }
                if (bools[1])
                {
                    Position temp = new Position(thePiece.position.x - i, thePiece.position.y + i);
                    bishopPositions.Add(temp);
                    if (IsPostitionOccupied(temp))
                        bools[1] = false;
                }
                if (bools[2])
                {
                    Position temp = new Position(thePiece.position.x - i, thePiece.position.y - i);
                    bishopPositions.Add(temp);
                    if (IsPostitionOccupied(temp))
                        bools[2] = false;
                }
                if (bools[3])
                {
                    Position temp = new Position(thePiece.position.x + i, thePiece.position.y - i);
                    bishopPositions.Add(temp);
                    if (IsPostitionOccupied(temp))
                        bools[3] = false;
                }
            }
            for (int i = 0; i < bishopPositions.Count; i++)
            {
                CheckSurroundings(thePiece, bishopPositions[i]);
            }
        } // BishopMover
        public static void KnightMover(ChessPiece thePiece)
        {
            Position[] knightPositions =
            {
                new Position(thePiece.position.x + 2, thePiece.position.y + 1),
                new Position(thePiece.position.x + 2, thePiece.position.y - 1),
                new Position(thePiece.position.x - 2, thePiece.position.y + 1),
                new Position(thePiece.position.x - 2, thePiece.position.y - 1),
                new Position(thePiece.position.x + 1, thePiece.position.y + 2),
                new Position(thePiece.position.x - 1, thePiece.position.y + 2),
                new Position(thePiece.position.x + 1, thePiece.position.y - 2),
                new Position(thePiece.position.x - 1, thePiece.position.y - 2),
            };
            for (int i = 0; i < knightPositions.Length; i++)
            {
                CheckSurroundings(thePiece, knightPositions[i]);

            }
        } // KnightMover
        public static void RookMover(ChessPiece thePiece)
        {
            List<Position> rookPositions = new List<Position>();
            bool[] bools = new bool[4];
            for (int i = 0; i < bools.Length; i++)
                bools[i] = true;

            for (int i = 1; i < 8; i++)
            {
                if (bools[0])
                {
                    Position temp = new Position(thePiece.position.x + i, thePiece.position.y);
                    rookPositions.Add(temp);
                    if (IsPostitionOccupied(temp))
                        bools[0] = false;
                }
                if (bools[1])
                {
                    Position temp = new Position(thePiece.position.x - i, thePiece.position.y);
                    rookPositions.Add(temp);
                    if (IsPostitionOccupied(temp))
                        bools[1] = false;
                }
                if (bools[2])
                {
                    Position temp = new Position(thePiece.position.x, thePiece.position.y + i);
                    rookPositions.Add(temp);
                    if (IsPostitionOccupied(temp))
                        bools[2] = false;
                }
                if (bools[3])
                {
                    Position temp = new Position(thePiece.position.x, thePiece.position.y - i);
                    rookPositions.Add(temp);
                    if (IsPostitionOccupied(temp))
                        bools[3] = false;
                }
            }
            for (int i = 0; i < rookPositions.Count; i++)
            {
                CheckSurroundings(thePiece, rookPositions[i]);
            }
        } // RookMover
        public static void PawnMover(ChessPiece thePiece)
        {
            List<Position> pawnPostions = new List<Position>();
            int polarrization = 1;
            if (!thePiece.isWhite)
                polarrization = -1;

            // checking the varoius possible postions for pawns
            // checking if the spot straight ahead is takable or not
            Position tempPos1 = new Position(thePiece.position.x, thePiece.position.y + 1 * polarrization);
            if (!IsPieceTakeable(tempPos1, thePiece.isWhite))
                pawnPostions.Add(tempPos1);

            // checking if it can move two spots
            Position tempPos2 = new Position(thePiece.position.x, thePiece.position.y + 2 * polarrization);
            if (!IsPostitionOccupied(tempPos1) && !IsPostitionOccupied(tempPos2))
            {
                if (thePiece.isWhite)
                {
                    if (thePiece.position.y == 1)
                        pawnPostions.Add(tempPos2);
                }
                else
                {
                    if (thePiece.position.y == 6)
                        pawnPostions.Add(tempPos2);
                }
            }

            // attack position to the right
            Position tempPos3 = new Position(thePiece.position.x + 1 * polarrization, thePiece.position.y + 1 * polarrization);
            if (IsPieceTakeable(tempPos3, thePiece.isWhite))
                pawnPostions.Add(tempPos3);

            // attack position to the left
            Position tempPos4 = new Position(thePiece.position.x - 1 * polarrization, thePiece.position.y + 1 * polarrization);
            if (IsPieceTakeable(tempPos4, thePiece.isWhite))
                pawnPostions.Add(tempPos4);

            // an attempt at making en passant
            if (thePiece.isWhite && thePiece.position.y == 4 || !thePiece.isWhite && thePiece.position.y == 3)
            {
                // en passant check on the right side
                Position tempEnpassant1 = new Position(thePiece.position.x + 1 * polarrization, thePiece.position.y + 1 * polarrization);
                Position tempEnpassant2 = new Position(thePiece.position.x + 1 * polarrization, thePiece.position.y);
                if (IsPieceTakeable(tempEnpassant2, thePiece.isWhite) && !IsPostitionOccupied(tempEnpassant1) && MainPage.piecePostions[MainPage.PositionToString(tempEnpassant2)].lastMoveRound == MainPage.currentRound - 1)
                    CheckSurroundings(thePiece, tempEnpassant2, tempEnpassant1);

                // en passant check on the left side
                Position tempEnpassant3 = new Position(thePiece.position.x - 1 * polarrization, thePiece.position.y + 1 * polarrization);
                Position tempEnpassant4 = new Position(thePiece.position.x - 1 * polarrization, thePiece.position.y);
                if (IsPieceTakeable(tempEnpassant4, thePiece.isWhite) && !IsPostitionOccupied(tempEnpassant3) && MainPage.piecePostions[MainPage.PositionToString(tempEnpassant4)].lastMoveRound == MainPage.currentRound - 1)
                    CheckSurroundings(thePiece, tempEnpassant4, tempEnpassant3);
            }



            // checking the possible postions
            for (int i = 0; i < pawnPostions.Count; i++)
            {
                CheckSurroundings(thePiece, pawnPostions[i]);
            }
        } // PawnMover
        private static void CheckSurroundings(ChessPiece thePiece, Position positionToCheck, Position enPassant)
        {
            if (positionToCheck.x >= 0 && positionToCheck.x <= 7 && positionToCheck.y >= 0 && positionToCheck.y <= 7)
            {
                string passOn = string.Format(MainPage.PositionToString(thePiece.position) + "|" + MainPage.PositionToString(positionToCheck) + "|" + MainPage.PositionToString(enPassant));
                Button button = new Button
                {
                    Opacity = 0,
                    ZIndex = 30,
                    ClassId = passOn,

                };
                Button button2 = new Button
                {
                    Opacity = 0,
                    ZIndex = 30,
                    ClassId = passOn,

                };
                Button border = new Button
                {
                    BorderColor = Colors.Red,
                    BorderWidth = 5,
                    ZIndex = 15,
                };
                MainPage.Board.Children.Add(border);
                Grid.SetColumn(border, enPassant.x);
                Grid.SetRow(border, enPassant.y);
                removeLater.Add(border);

                MainPage.Board.Children.Add(button);
                Grid.SetColumn(button, enPassant.x);
                Grid.SetRow(button, enPassant.y);
                removeLater.Add(button);

                MainPage.Board.Children.Add(button2);
                Grid.SetColumn(button2, positionToCheck.x);
                Grid.SetRow(button2, positionToCheck.y);
                removeLater.Add(button2);

                button.Clicked += TakePiece_Clicked;
                button2.Clicked += TakePiece_Clicked;
            }
        } // CheckSurroundings
        private static void TakePiece(Position enemyPosition, ChessPiece pieceTryingToTake, Position enPassant)
        {
            // "killing" the enemy piece
            MainPage.piecePostions[MainPage.PositionToString(enemyPosition)].isAlive = false;
            MainPage.Board.Remove(MainPage.imageButtons[MainPage.piecePostions[MainPage.PositionToString(enemyPosition)].index]);

            MainPage.piecePostions.Remove(MainPage.PositionToString(enemyPosition));


            MovePiece(pieceTryingToTake.position, enPassant);
        } // TakePiece
    }
}
