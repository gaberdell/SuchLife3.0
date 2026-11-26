##### Quick Document to help me organize all I need to do this week



1. Check and fix all Bugs on my branch
2. Merge in all branches
3. Check for bugs now with branches merged
4. Potentially add new features



Check and fix all Bugs on my branch

* Check if player can join server :white_check_mark:
  * Check if player can see player addition
  * Check if player can see player prefab addition
* Check if players can move in parallel

  * Check if prefab is moved over in update
  * Double check that player doesn't accidentally send UUID as update
* Check if players moving causes that player to move or only one another
* Check rejoin functionality
* Check deletion functionality

  * Create something that potentially gets deleted as nothing currently does so otherwise won't get tested



Merge in all branches

(When merging switch to other branch then merge in main thru there to resolve merge conflicts locally)

* Merge in Kayla-ArtUI
* Merge in mac\_documentation
* Merge in cole-multiFixes branch
* Merge in lucas-changes
* Double check no other branches need to be merged in



Check for bugs now with branches merged

(I.E. checking if regressions were created)

* Check loading into world
* Check world creation and deletion
* Player

  * Does player move?
  * Can player punch enemies?
  * Can player craft?
  * Can player destroy block
  * Can player place block
* Enemy

  * Can enemy follow player
  * Can enemy be dealt damage
  * Can enemy die
  * Will enemy attempt to attack
* Save/Load

  * Does entity state save
  * Does block state save
* Multiplayer

  * Can player join server
  * Can player see other player join
  * Can player move in parallel
  * Can player rejoin
  * Can player see deletion



Potentially add new features

* Save/Load

  * Add inventory saving
  * Add basic entities to world when initial created to save
* Multiplayer

  * Extended player controls protocol (placing/destroying, used item, selected item, slot)
  * Send inventory updates
  * Send block updates

    * Make quick UDP update protocol
    * Make concrete TCP protocol to be sent every once in a while



