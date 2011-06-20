// Copyright (c) 2010 Art & Logic, Inc. All Rights Reserved.
// $Id: $

function createCookie(name, value)
{
   document.cookie = name + "=" + value + "; path=/";
}

function readCookie(name)
{
   var nameEQ = name + "=";
   var ca = document.cookie.split(';');
   for(var i = 0; i < ca.length; ++i)
   {
      var c = ca[i];
      while (c.charAt(0) == ' ')
      {
         c = c.substring(1, c.length);
      }
      if (c.indexOf(nameEQ) == 0)
      {
         return c.substring(nameEQ.length, c.length);
      }
   }
   return null;
}

function AMatchThreeLeaderboard(view)
{
   this.fView = view;
   this.fScores = []; // 0 (lowest) -> 10 (highest),
                      // { player: 'Bob', score: 1250 }

   this.Load();
}

AMatchThreeLeaderboard.prototype.Scores = function()
{
   return this.fScores;
};

AMatchThreeLeaderboard.prototype.IsTopTen = function(score)
{
   if (!this.fScores || this.fScores.length < 10)
   {
      return true;
   }
   return (score >= this.fScores[0].score);
};

AMatchThreeLeaderboard.prototype.Record = function(player, score)
{
   if (!this.fScores || this.fScores.length == 0)
   {
      this.fScores = [{ player: player, score: score }];
   }
   else if (this.fScores.length < 10)
   {
      // until the board is full, push scores up
      for (var i = 0; i <= this.fScores.length; ++i)
      {
         if (i == this.fScores.length)
         {
            // high score so far
            this.fScores.push({ player: player, score: score });
            break;
         }
         if (score < this.fScores[i].score)
         {
            for (var j = this.fScores.length-1; j >= i; --j)
            {
               this.fScores[j+1] = this.fScores[j];
            }
            this.fScores[i] = { player: player, score: score };
            break;
         }
      }
   }
   else
   {
      // once the board is full, push scores down
      for (var i = this.fScores.length - 1; i >= 0; --i)
      {
         if (score >= this.fScores[i].score)
         {
            for (j = 0; j < i; ++j)
            {
               this.fScores[j] = this.fScores[j+1];
            }
            this.fScores[i] = { player: player, score: score };
            break;
         }
      }
   }
   this.Save();
   this.Update();
};

AMatchThreeLeaderboard.prototype.Load = function()
{
   this.fScores = [];
   var a = readCookie('match3-leaderboard');
   if (a)
   {
      var parts = a.split(',');
      for (var i = 0; i < parts.length; i += 2)
      {
         this.fScores.push({
            player: parts[i],
            score: Number(parts[i+1])
         });
      }
   }
   this.Update();
};

AMatchThreeLeaderboard.prototype.Save = function()
{
   var parts = [];
   for (var i = 0; i < this.fScores.length; ++i)
   {
      parts.push(this.fScores[i].player);
      parts.push(this.fScores[i].score);
   }
   createCookie('match3-leaderboard', parts.join(','));
};

AMatchThreeLeaderboard.prototype.Update = function()
{
   var table = $(this.fView);
   table.empty();
   if (this.fScores.length == 0)
   {
      table.append('<tr><td colspan="2">No high scores yet, keep playing!</tr>');
   }
   for (var i = this.fScores.length-1; i >= 0; --i)
   {
      table.append(
         '<tr><td>' + (this.fScores.length - i) +
         '</td><td>' + this.fScores[i].player +
         '</td><td>' + this.fScores[i].score +
         '</td></tr>'
      );
   }
};

function AMatchThreeScore(scoreboard, timeAllowed)
{
   this.fTimeGameStarted = 0;
   this.fTimeAllowed = timeAllowed * 1000;
   this.fScoreBoard = scoreboard;
   this.fMoves = 0;
   this.fMatches = [];
   this.fMatches[3] = 0;
   this.fMatches[4] = 0;
   this.fMatches[5] = 0;
   this.fColors = {};
   this.fPoints = 0;
   this.fLongestCascade = 0;
   this.fStarted = new Date();

   if (this.fScoreBoard)
   {
      $(this.fScoreBoard).addClass('match3-score');
   }
   this.Update();
}

AMatchThreeScore.prototype.GetScore = function()
{
   return this.fPoints;
};

AMatchThreeScore.prototype.Update = function()
{
   // update display
   if (this.fScoreBoard)
   {
      $(this.fScoreBoard).find('.score-points').text(this.fPoints);
      $(this.fScoreBoard).find('.score-3').text(this.fMatches[3]);
      $(this.fScoreBoard).find('.score-4').text(this.fMatches[4]);
      $(this.fScoreBoard).find('.score-5').text(this.fMatches[5]);
      $(this.fScoreBoard).find('.score-moves').text(this.fMoves);
      $(this.fScoreBoard).find('.score-cascades').text(this.fLongestCascade);
   }
   this.UpdateTime();
};

AMatchThreeScore.prototype.GetTimeRemaining = function()
{
   if (0 == this.fTimeGameStarted) return 0;
   return this.fTimeAllowed - new Date().getTime() + this.fTimeGameStarted;
};

AMatchThreeScore.prototype.UpdateTime = function()
{
   if (this.fScoreBoard)
   {
      if (0 == this.fTimeGameStarted)
      {
         $('.score-time').text('-');
      }
      else
      {
         // calculate time remaining in seconds
         var ms = this.GetTimeRemaining();
         var s = Math.floor(ms / 1000);

         // get minutse and seconds breakdown
         var mins = Math.floor(s / 60).toString();
         var secs = Math.floor(s % 60).toString();

         // add leading zeros
         if (mins.length < 2)
         {
            mins = '0' + mins;
         }
         if (secs.length < 2)
         {
            secs = '0' + secs;
         }
         
         $('.score-time').text(mins + ':' + secs);
      }
   }
};

AMatchThreeScore.prototype.Move = function()
{
   this.fMoves++;
   this.Update();
};

AMatchThreeScore.prototype.Score = function(matches, cascade)
{
   if (0 == this.fTimeGameStarted)
   {
      this.fTimeGameStarted = new Date().getTime();
   }

   var multiplier = 10 + (cascade * 5);
   for (var i = 0; i < matches.length; ++i)
   {
      // tally the match
      var m = matches[i];
      if (null == this.fMatches[m.length])
      {
         this.fMatches[m.length] = 0;
      }
      if (null == this.fColors[m.color])
      {
         this.fColors[m.color] = 0;
      }
      this.fMatches[m.length]++;
      this.fColors[m.color]++;

      // score the match
      var points = ({3: 10, 4: 25, 5: 75})[m.length] * multiplier;
      this.fPoints += points;
      
      // compute additional time from points, baseline is a single 3-match
      // is worth a base of 1 second (100 points -> 1 second)
      this.fTimeAllowed += points * 10;
   }

   if (cascade > this.fLongestCascade)
   {
      this.fLongestCascade = cascade;
   }

   this.Update();
};

function AMatchThreeTile(color)
{
   this.fColor = color;
   this.fView = jQuery('<div class="tile"></div>')
                  .data('tile', this)
                  .addClass('tile-' + color);   
}

AMatchThreeTile.prototype.Select = function (selected)
{
   this.fView.toggleClass('selected', selected);
};

AMatchThreeTile.prototype.GetView = function ()
{
   return this.fView;
};

AMatchThreeTile.prototype.GetColor = function ()
{
   return this.fColor;
};

AMatchThreeTile.prototype.SetColor = function (color)
{
   this.fView.removeClass('tile-' + this.fColor);
   this.fColor = color;
   this.fView.addClass('tile-' + this.fColor);
};

AMatchThreeTile.prototype.MoveTo = function (viewportLocation)
{
   this.fView.css({
      left: viewportLocation[0] + 'px',
      top: viewportLocation[1] + 'px'
   });
};

AMatchThreeTile.prototype.AnimateTo = function (viewportLocation, duration)
{
   if (!duration)
   {
      duration = 600;
   }

   this.fView.animate(
      {
         left: viewportLocation[0] + 'px',
         top: viewportLocation[1] + 'px'
      },
      duration
   );
};

function AMatchThreeApplication(board, settings)
{
   // build config settings
   this.fConfig =
      {
         gameDurationInSeconds: 120, // length of game
         boardSize:        [ 8,  8], // size of the board in tiles
         boardPadding:     [10, 10], // padding around the gameboard.. css padding doesnt work
         tileSize:         [63, 63], // size of a tile in pixels
         tileSpacing:    [ -1,  -1], // padding between two adjascent tiles, in pixels
         tileCount:               6  // how many unique tile types to use
      };
   if (settings)
   {
      jQuery.extend(this.fConfig, settings);
   }
   
   // save board
   this.fBoard = jQuery(board)
      .data('game', this)
      .addClass('match3-board');

   this.fSelected = null;
   this.fCascade = 0;
   this.fState = AMatchThreeApplication.StateReady;
   this.fScore = null;
   
   // populate initial board
   this.ResetBoard();

   // begin updating time display
   this.OnTimerPulse();
}

/*

Game states & transitions:

0: Ready -> TileSelected
1: TileSelected -> TilesSwapping | Ready (Illegal move, shaking animation)
2: TilesSwappineg -> TilesClearing (Clear animation, call to scoring)
3: TilesClearing -> TilesRefilling
4: TilesRefilling -> TilesClearing (New tiles create additional matches) | Ready

*/
AMatchThreeApplication.StateReady = 0;
AMatchThreeApplication.StateTileSelected = 1;
AMatchThreeApplication.StateTilesSwapping = 10;
AMatchThreeApplication.StateTilesClearing = 11;
AMatchThreeApplication.StateTilesRefilling = 12;
AMatchThreeApplication.StateGameOver = 20;

AMatchThreeApplication.prototype.GetState = function ()
{
   return this.fState;
};

AMatchThreeApplication.prototype.SetState = function (state)
{
   this.fState = state;
};

AMatchThreeApplication.prototype.GameOver = function ()
{
   var go = this.fConfig.onGameover;
   if (go)
   {
      this.SetState(AMatchThreeApplication.StateGameOver);
      go(this.fScore.GetScore());
   }
   else
   {
      // no overlay, just reset the board
      this.ResetBoard();
   }
};


AMatchThreeApplication.prototype.OnTimerPulse = function ()
{
   if (this.GetState() != AMatchThreeApplication.StateGameOver)
   {
      if (this.fScore.GetTimeRemaining() < 0)
      {
         this.GameOver();
      }
      else
      {
         this.fScore.UpdateTime();
      }
   }

   // repeat every tenth of a second
   var game = this;
   setTimeout(function()
      {
         game.OnTimerPulse();
      },
      100);
};

AMatchThreeApplication.prototype.GetBoardLocation = function (tile)
{
   for (var x = 0; x < this.fTiles.length; ++x)
   {
      if (this.fTiles[x] == tile)
      {
         return [
            x % this.fConfig.boardSize[0],
            Math.floor(x / this.fConfig.boardSize[0])
         ];
      }
   }
   throw "Invalid game state (cannot find tile)";
};

AMatchThreeApplication.prototype.GetTile = function(location)
{
   return this.fTiles[
      location[1] * this.fConfig.boardSize[0] + 
      location[0]
   ];
};

AMatchThreeApplication.prototype.SetTile = function(location, tile)
{
   this.fTiles[
      location[1] * this.fConfig.boardSize[0] +
      location[0]
   ] = tile;
};

AMatchThreeApplication.prototype.IsAdjacent = function (locationA, locationB)
{
   return 1 == Distance(locationA, locationB);
};


AMatchThreeApplication.prototype.CanActivateTile = function (tile)
{
    // check for valid states
   if (this.GetState() != AMatchThreeApplication.StateReady &&
       this.GetState() != AMatchThreeApplication.StateTileSelected)
   {
      // just ignore clicks during swapping, clearing, and filling
      return false;
   }
   return true;
}

AMatchThreeApplication.prototype.OnTileClicked = function (tile)
{
   if (!this.CanActivateTile())
   {
      return;
   }

   // if no current selection, simply highlight this tile
   if (!this.fSelected)
   {
      tile.Select(true);
      this.fSelected = tile;
      this.SetState(AMatchThreeApplication.StateTileSelected);
      return;
   }

   // cancel the selection if this tile already selected
   if (tile == this.fSelected)
   {
      this.fSelected.Select(false);
      this.fSelected = null;
      this.SetState(AMatchThreeApplication.StateReady);
      return;
   }

   // get coordinate of current and target tiles
   var tileA = this.fSelected;
   var tileB = tile;
   var locationA = this.GetBoardLocation(tileA);
   var locationB = this.GetBoardLocation(tileB);

   // transfer selection if a non-adjacent tile was selected
   if (!this.IsAdjacent(locationA, locationB))
   {
      this.fSelected.Select(false);
      this.fSelected = tileB;
      this.fSelected.Select(true);
      return;
   }

   this.SwapTiles(tileA, tileB, locationA, locationB)
}


AMatchThreeApplication.prototype.SwapTiles = function (tileA, tileB, locationA, locationB)
{
   // swap the tiles in our state and check for matches
   this.SetTile(locationA, tileB);
   this.SetTile(locationB, tileA);

   // find matches
   var matches = this.FindMatches();
   if (matches.length == 0)
   {
      // no matches.. illegal move. move the tiles back
      this.SetTile(locationA, tileA);
      this.SetTile(locationB, tileB);
      if (this.fSelected)
      {
         this.fSelected.Select(false);
         this.fSelected.GetView().shake();
         this.fSelected = null;
      }
      tileB.GetView().shake();
      if (this.fConfig.onSwapDenied)
      {
         this.fConfig.onSwapDenied();
      }
      return false;
   }

   // unselect
   if (this.fSelected)
   {
      this.fSelected.Select(false);
   }
   this.fSelected = null;

   // animate swap a bit more slowly than regular tile movement
   tileA.AnimateTo(this.BoardToPixel(locationB), tileVelocity * 2);
   tileB.AnimateTo(this.BoardToPixel(locationA), tileVelocity * 2);

   // wait to clear matches till swap animation finishes
   var game = this;
   setTimeout(
      function()
      {
         game.fScore.Move();
         game.ClearMatches(matches);
      },
      tileVelocity * 2
   );

   if (this.fConfig.onSwapAllowed)
   {
      this.fConfig.onSwapAllowed();
   }

   return true;
};

var tileVelocity = 200; // milliseconds/square

AMatchThreeApplication.prototype.ClearMatches = function (matches)
{
   if (!matches)
   {
      matches = this.FindMatches();
   }
   if (matches.length == 0)
   {
      this.fCascade = 0;

      if (this.fConfig.onMatchesFinished)
      {
         this.fConfig.onMatchesFinished();
      }

      // !!! add  '|| true' to see a game over after the first move
      // see if there are any moves left on the board
      var move = this.FirstAvailableMove();
      if (!move)
      {
         this.GameOver();
         return;
      }

      this.SetState(AMatchThreeApplication.StateReady);
      return;
   }
   this.SetState(AMatchThreeApplication.StateTilesClearing);

   // union all matched tiles (single tile often part of more than one match)
   var tiles = [];
   for (var i = 0; i < matches.length; ++i)
   {
      var a = this.ExpandMatch(matches[i]);
      for (var j = 0; j < a.length; ++j)
      {
         if (!LocationInArray(tiles, a[j]))
         {
            tiles.push(a[j]);
         }
      }
   }

   // apply the match animation and remove from board
   for (var t = 0; t < tiles.length; ++t)
   {
      this.GetTile(tiles[t]).GetView().addClass(
         'matched',
         500,
         function ()
         {
            $(this).remove();
         }
      );
      this.SetTile(tiles[t], null);
   }

   // apply scores
   this.fScore.Score(matches, this.fCascade);
   this.fCascade++;

   var game = this;
   setTimeout(
      function()
      {
         game.RefillBoard(tiles);
      },
      600
   );
};

AMatchThreeApplication.prototype.RefillBoard = function (tiles)
{
   // move across columns shifting blocks down, then filling the
   // new empty space at the top with fresh blocks
   this.SetState(AMatchThreeApplication.StateTilesRefilling);
   for (var x = 0; x < this.fConfig.boardSize[0]; ++x)
   {
      // push all the y coords of matched tiles (and now gone)
      var tilesInCol = [];
      for (var t = 0; t < tiles.length; ++t)
      {
         if (tiles[t][0] == x)
         {
            tilesInCol.push(tiles[t][1]);
         }
      }

      // shift existing blocks down
      for (var y = this.fConfig.boardSize[1] -1; y >= 0; --y)
      {
         var tile = this.GetTile([x, y]);
         if (tile)
         {
            // count emptied tiles below us
            var pushdown = 0;
            for (var t = 0; t < tilesInCol.length; ++t)
            {
               if (tilesInCol[t] > y)
               {
                  pushdown++;
               }
            }

            // tile velocity is 1/200 sq/ms
            if (pushdown > 0)
            {
               this.SetTile([x, y], null);
               this.SetTile([x, y+pushdown], tile);
               tile.AnimateTo(
                     this.BoardToPixel([x, y+pushdown]),
                     pushdown * tileVelocity
               );
            }
         }
      }

      // fill new blocks from the top, delay the animation so
      // that there is always only the one more block scrolling in
      // from the top

      var longestDuration = 0;
      for (y = 0; y < tilesInCol.length; ++y)
      {
         var duration = tilesInCol.length * tileVelocity;
         if (duration > longestDuration)
         {
            longestDuration = duration;
         }

         tile = this.GenerateTile();
         tile.MoveTo(this.BoardToPixel([x, y - tilesInCol.length - 1]));
         this.SetTile([x, y], tile);
         this.AddTileToView(tile);
         tile.AnimateTo(
            this.BoardToPixel([x, y]),
            duration
         );
      }
   }

   // keep looking for matches.. ClearMatches() has the escape clauses
   var game = this;
   setTimeout(
      function()
      {
         game.ClearMatches();
      },
      longestDuration + 100
   );
};

AMatchThreeApplication.prototype.GenerateTile = function ()
{
   var randomColor = Math.floor(Math.random() * this.fConfig.tileCount);
   return new AMatchThreeTile(randomColor);
};

/**
  * Reset/populate the playfield with random tiles.
  */
AMatchThreeApplication.prototype.ResetBoard = function ()
{
   if (this.GetState() != AMatchThreeApplication.StateReady &&
       this.GetState() != AMatchThreeApplication.StateTileSelected &&
       this.GetState() != AMatchThreeApplication.StateGameOver )
   {
      return;
   }

   // reset score
   this.fScore = new AMatchThreeScore(this.fConfig.scoreBoard, this.fConfig.gameDurationInSeconds);

   // reset the board will randomize tiles
   this.fBoard.find('.tile').remove();
   this.fTiles = new Array(this.fConfig.boardSize[0] * this.fConfig.boardSize[1]);
   for (var x = 0; x < this.fConfig.boardSize[0] * this.fConfig.boardSize[1]; ++x)
   {
      this.fTiles[x] = this.GenerateTile();
   }

   // clear matches from board & refill until a stable board is found
   var limiter = 10000;
   var matches = this.FindMatches();
   while (--limiter > 0)
   {
      for (var i = 0; i < matches.length; ++i)
      {
         var tiles = this.ExpandMatch(matches[i]);
         for (var j = 0; j < tiles.length; ++j)
         {
            var t = this.GetTile(tiles[j]);
            t.SetColor(Math.floor(Math.random() * this.fConfig.tileCount));
         }
      }
      matches = this.FindMatches();
      if (matches.length == 0 &&
          this.FirstAvailableMove())
      {
         break;
      }

   }

   // push tiles to the screen
   for (x = 0; x < this.fConfig.boardSize[0]; ++x)
   {
      for (var y = 0; y < this.fConfig.boardSize[1]; ++y)
      {
         t = this.GetTile([x, y]);
         this.AddTileToView(t);
         t.MoveTo(this.BoardToPixel([x, (-1 * this.fConfig.boardSize[1]) - (2 * y * Math.random())]));

         t.AnimateTo(this.BoardToPixel([x, y], tileVelocity * (20/y)))
      }
   }

   this.fBoard.find('.match3-overlay').hide();
   this.fSelected = null;
   this.SetState(AMatchThreeApplication.StateReady);
};

AMatchThreeApplication.prototype.AddTileToView = function (t)
{
   this.fBoard.append(t.GetView());
   t.GetView().draggable({
      distance: 3,
      containment: 'parent',
      start: function(event, ui)
      {
         var game = ui.helper.closest('.match3-board').data('game');
         if (!game.CanActivateTile())
         {
            return false;
         }
         ui.helper.css({'z-index': 2});
      },
      stop:  function (event, ui)
      {
         $('.drop-over').removeClass('drop-over');
         ui.helper.css({'z-index': 0});

         var game = ui.helper.closest('.match3-board').data('game');
         var tileA = ui.helper.data('tile');
         var tileB = game.HitTest([
            ui.position.left + Math.floor(game.fConfig.tileSize[0] / 2),
            ui.position.top + Math.floor(game.fConfig.tileSize[1] / 2)
         ]);

         var locationA = game.GetBoardLocation(tileA);
         var locationB = game.GetBoardLocation(tileB);

         // cancel if non-adjacent
         if (!game.IsAdjacent(locationA, locationB) ||
             !game.SwapTiles(tileA, tileB, locationA, locationB))
         {
            ui.helper.css({
               left: ui.originalPosition.left + 'px',
               top: ui.originalPosition.top + 'px'
            });
         }
      },
      drag: function (event, ui)
      {
         var game = ui.helper.closest('.match3-board').data('game');
         var over = game.HitTest([
            ui.position.left + Math.floor(game.fConfig.tileSize[0] / 2),
            ui.position.top + Math.floor(game.fConfig.tileSize[1] / 2)
         ]);
         if (over && over != ui.helper)
         {
            var view = over.GetView();
            if (!view.hasClass('drop-over'))
            {
               $('.drop-over').removeClass('drop-over');
               over.GetView().addClass('drop-over');
            }
         }
      }
   });
}

AMatchThreeApplication.prototype.HitTest = function (viewportLocation)
{
   var boardLocation = [
      Math.max(0, Math.min(this.fConfig.boardSize[0] - 1, Math.floor((viewportLocation[0] - this.fConfig.boardPadding[0]) / (this.fConfig.tileSize[0] + this.fConfig.tileSpacing[0])))),
      Math.max(0, Math.min(this.fConfig.boardSize[1] - 1, Math.floor((viewportLocation[1] - this.fConfig.boardPadding[1]) / (this.fConfig.tileSize[1] + this.fConfig.tileSpacing[1]))))
   ];
   return this.GetTile(boardLocation);
};

/*
 * Expand a match (with start: and end: coords) into an array of
 * individual tile locations.
 */
AMatchThreeApplication.prototype.ExpandMatch = function (match)
{
   var a = match.start;
   var b = match.end;
   var tiles = [];
   if (a[0] == b[0])
   {
      for (var y = a[1]; y <= b[1]; ++y)
      {
         tiles.push([a[0], y]);
      }
   }
   if (a[1] == b[1])
   {
      for (var x = a[0]; x <= b[0]; ++x)
      {
         tiles.push([x, a[1]]);
      }
   }
   return tiles;
};

AMatchThreeApplication.prototype.FindMatches = function ()
{
   var matches = [];
   function PushMatch(a, b, color)
   {
      // given the tiles:
      // [blue, blue, blue]
      // the distance is 2, but the
      // run length is 3; a match.
      var d = Distance(a, b);
      if (d >= 2)
      {
         matches.push({
            start: a,
            end: b,
            length: d + 1,
            color: color
         });
      }
   }

   // scan left to right in rows
   for (var y = 0; y < this.fConfig.boardSize[1]; ++y)
   {
      var matchBegins = [0, y];
      var matchColor = this.GetTile(matchBegins).GetColor();
      for (var x = 1; x < this.fConfig.boardSize[0]; ++x)
      {
         var thisColor = this.GetTile([x, y]).GetColor();
         if (matchColor != thisColor)
         {
            var matchEnds = [x - 1, y];
            PushMatch(matchBegins, matchEnds, matchColor);

            matchBegins = [x, y];
            matchColor = thisColor;
         }
      }

      // try to finish a match at the end of the row
      PushMatch(matchBegins, [this.fConfig.boardSize[0] - 1, y], matchColor);
   }

   // scan top to bottom in cols
   for (x = 0; x < this.fConfig.boardSize[0]; ++x)
   {
      matchBegins = [x, 0];
      matchColor = this.GetTile(matchBegins).GetColor();
      for (y = 1; y < this.fConfig.boardSize[1]; ++y)
      {
         thisColor = this.GetTile([x, y]).GetColor();
         if (matchColor != thisColor)
         {
            matchEnds = [x, y - 1];
            PushMatch(matchBegins, matchEnds, matchColor);

            matchBegins = [x, y];
            matchColor = thisColor;
         }
      }

      // try to finish a match at the bottom of a col
      PushMatch(matchBegins, [x, this.fConfig.boardSize[1] - 1], matchColor);
   }

   return matches;
};

AMatchThreeApplication.prototype.TrySwap = function(locationA, locationB)
{
   var tileA = this.GetTile(locationA);
   var tileB = this.GetTile(locationB);
   this.SetTile(locationA, tileB);
   this.SetTile(locationB, tileA);
   var matches = this.FindMatches();
   this.SetTile(locationA, tileA);
   this.SetTile(locationB, tileB);
   return matches;
}

/*
 * Scan board for an available move
 */
AMatchThreeApplication.prototype.FirstAvailableMove = function ()
{
   // swaps are commutative, such that [0, 0] to [1, 0] is the same as
   // [1, 0] to [0, 0]. so by convention we swap only to the right and
   // only downward, and hence the last row/col is not checked.. it has
   // no unique moves that werent checked by the previous row/col.
   function TryMove(app, locationA, locationB)
   {
      var matches = app.TrySwap(locationA, locationB);
      if (matches.length > 0)
      {
         return {
            from: locationA,
            to: locationB,
            matches: matches
         };
      }
      return null;
   }

   for (var y = 0; y < this.fConfig.boardSize[1] - 1; ++y)
   {
      for (var x = 0; x < this.fConfig.boardSize[0] - 1; ++x)
      {
         // check to the right, then down
         var move = TryMove(this, [x, y], [x+1, y]) ||
                    TryMove(this, [x, y], [x, y+1]);
         if (move)
         {
            return move;
         }
      }
   }

   // no moves!
   return null;
};

/**
  * Calculate viewport relative pixels coordinates from board coordinates.
  * \param boardLocation Coordinates on the board, in the form [x, y].
  */
AMatchThreeApplication.prototype.BoardToPixel = function (boardLocation)
{
return [
         this.fConfig.boardPadding[0] + (boardLocation[0] * this.fConfig.tileSize[0]) + (boardLocation[0] * this.fConfig.tileSpacing[0]),
         this.fConfig.boardPadding[1] + (boardLocation[1] * this.fConfig.tileSize[1]) + (boardLocation[1] * this.fConfig.tileSpacing[1])
   ];
};


function Distance(locationA, locationB)
{
if (locationA[0] == locationB[0])
{
   return Math.abs(locationA[1] - locationB[1]);
}
if (locationA[1] == locationB[1])
{
   return Math.abs(locationA[0] - locationB[0]);
}
return NaN;
}

function LocationInArray(arr, location)
{
   for (var i = 0; i < arr.length; ++i)
   {
      var a = arr[i];
      if (a[0] == location[0] &&
          a[1] == location[1])
      {
         return true;
      }
   }
   return false;
}

var lastHint = 0;

(function ($)
{
   // install $().match3({ settings... }); helper
   $.fn.match3 = function (settings)
   {
      this.each(function()
      {
         if (typeof(settings) == 'string')
         {
            var app = $(this).data('game');
            if (settings == 'hint')
            {
               if (app.GetState() == AMatchThreeApplication.StateReady ||
                   app.GetState() == AMatchThreeApplication.StateTileSelected)
               {
                  if (new Date().getTime() - lastHint > 3000)
                  {
                     var move = app.FirstAvailableMove();
                     app.GetTile(move.from).GetView().highlight();
                     app.GetTile(move.to).GetView().highlight();
                     lastHint = new Date().getTime();
                  }
               }
            }
            else if (settings == 'reset')
            {
               app.ResetBoard();
            }
         }
         else
         {
            new AMatchThreeApplication(this, settings);
         }
      });
   };

   // thanks to:
   // http://www.shesek.info/web-development/jquery-shake-function
   // note: jquery ui effects aren't playing nicely with our board,
   // resorting to handrolled animations
   $.fn.shake = function(opt)
   {
      opt = $.extend({times: 8, delay: 50, pixels: 6},opt||{});
      $(this).each(function()
      {
         var orig = parseInt($(this).css('left'));
         for (var i = 0; i < opt.times; ++i)
         {
            $(this).animate({left:orig+(opt.pixels*(i%2==0?1:-1))},opt.delay);
         }
         $(this).animate({left:orig},opt.delay);
      });
   }

   $.fn.highlight = function()
   {
      $(this).each(function()
      {
         var el = this;
         $(el).addClass('highlight', 200, function()
         {
            setTimeout(
               function()
               {
                  $(el).removeClass('highlight', 500);
               },
               1000);   
         });
      });
   };

   // install handler for all tile clicks
   $(function() 
   {
      $('.match3-board > .tile').live('click', function()
      {
         // propagate tile click to owning game object
         var tile = $(this).data('tile');
         var game = $(this).parent().data('game');
         game.OnTileClicked(tile);
      });
   });

})(jQuery);