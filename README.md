# Bubble Pops (2D, Mobile)
<p align="center"><img src="https://i.imgur.com/xG6zOhS.png" width="150" title="icon"></p>

## Table of Content
- [Description](#description)
- [Mechanics](#mechanics)
- [Visuals](#visuals)

## Description
It's a Puzzle game made with Unity. There are bubbles with numbers in them. Numbers start at 2 and all numbers are powers of two. Maximum number is 2024.

There is a board of bubbles. Player shoots random numbered bubbles to the board and try to match numbers. If there is a match, all surrounding numbers merge and make a new number which will also be power of two.
If a bubble reaches number 2024, it pops itself and surrounding bubbles. Board is like a spider web. If a bubble is not connected to the board, it falls.

Game is score based. There is no win or lose condition. If board comes too close to the bottom, board goes up to help player.

## Mechanics
- **Touch**\
  _Shooting is triggered with touch. Player just touches the screen and decides horizontal movement. Shooting power is constant._
- **Shooting**\
  _Player shoots bubbles to the board. Bubbles movement is shown with a line. Position bubble will go is also highlighted._
- **Merging Bubbles**\
  _If a thrown bubble matches a bubble on board, the same surrounding bubbles merge. Bubbles always try to merge somewhere upwards._
- **Bubble Pop**\
  _If a bubble becomes number 2024, it pops. It also make all surrounding bubbles pop._
- **Bubble Spawning**\
  _If bubble count on the boards is decreased, new line of bubbles will spawn and board will come down.
- **Board Going Up**\
  _There is no lose condition. So, if board comes too close to the bottom, it goes up to relieve the Player.__
- **Score**\
  _There is a multiplier for merging bubble count. It effects score. All bubble popping action gives Player score. It's shown on the top of the screen._

## Visuals
Here are some visuals;

Shooting                   | Merging Bubbles
:-------------------------:|:-------------------------:
![](https://media.giphy.com/media/v1.Y2lkPTc5MGI3NjExbzBjMG1rMzZpb2M0YnVlNndseTdzOW5qeDlianZ4b3kycm94MXlvOCZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/5C6wc3JavVXAoqAsy9/giphy.gif)  |  ![](https://media.giphy.com/media/v1.Y2lkPTc5MGI3NjExamJqdnRpbGUyaHMwMmJzY2lqeTdtbDV1M2wyMzQ1eXpheGI3cm5qZiZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/zLR6xMZCRQ8gvF775o/giphy.gif)

Bubbles Falling            | Bubbles Pop
:-------------------------:|:-------------------------:
![](https://media.giphy.com/media/v1.Y2lkPTc5MGI3NjExN2pvZWNsdTA4bngydXJncDliOHJydXBlMzVxNDJmazllYmEwbjM0dyZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/8R7qDOfSPzTCPiWOuM/giphy.gif)  |  ![](https://media.giphy.com/media/v1.Y2lkPTc5MGI3NjExNXlqdmpkbjY0cXJobWgweXBweHJuNjI2Nm05cm1vd2s4Zzd1c2lrYSZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/SdyxoWjOXFlRynhcFc/giphy.gif)

Board Going Up             | Menu
:-------------------------:|:-------------------------:
![](https://media.giphy.com/media/v1.Y2lkPTc5MGI3NjExeWF2cndoNnZuZmF1YzQ1cmQzeGo2NzZmempydDh4aGw4Nmh3cnR6NiZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/7E81aw4Dy9QT9Ovwdv/giphy.gif)  |  ![](https://media.giphy.com/media/v1.Y2lkPTc5MGI3NjExM2JrMjVlOW1jcDV3NjBzeWQ5Mjc2eXEwNWFocWJobmlxY3BrcmU4diZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/0MhNb47NsWo9JPNiti/giphy.gif)
