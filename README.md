Music Room is an audio player designed for loopable soundtrack such as video game loops, written by [Alice Jankowska](http://alphish.com/). To get the player, visit the [Music Room website](http://software.alphish.com/music-room/).

Multiple packages were used, in particular [NAudio](https://github.com/naudio/NAudio).

# Features

 - choosing looping parameters for a track
 - looping a track indefinitely
 - managing, saving and loading a playlist of loopable tracks
 - creating shareable playlists, independent from one's filesystem

# Changelog

 - **0.2.0.0** - initial release

# Project structure

At the moment, the Music Room code is split into two projects:

 - **Alphicsh.Audio** - playback functionality, such as a looping audio stream or an abstract audio player; since it's a pretty generic functionality, it's separated from the application itself
 - **Alphicsh.MusicRoom** - the actual application, along with its tracks and playlists structures