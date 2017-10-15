=================================================================
Music Room audio player
Copyright 2017 Alice Jankowska
See LICENSE.txt for full credits and license terms
=================================================================



Table of Contents
=================

- About
- Requirements
- Basic usage guide
	- Playlist management
	- Track management
	- Playback
- Additional remarks
	- Portability and shareability
	- Convenient playlist opening
- Changelog
- Useful links



About
=====

Music Room is an audio player designed for loopable soundtrack
such as video game loops. Unlike most audio players, it allows
the user to define looping points for specific tracks and store
them in a playlist. In particular, it can act as a lightweight
alternative to the sound test/jukebox modes known from games.



Requirements
============

You need to have installed .NET Framework 4.5.2 or higher
for the player to work correctly.

The framework is available here:
https://www.microsoft.com/en-us/download/details.aspx?id=42643

If your machine can run .NET Framework 4.5.2 in the first place,
it *should* be able to run Music Room without issues.



Basic usage guide
=================

Playlist management
-------------------

To add tracks to the playlist, use "File >> Add tracks" option
and choose one or more files to add. Alternatively, you can drag
supported audio files from the files browser and drop them
on the playlist area.

To remove tracks from the playlist, select one or more of them,
open the context menu (usually with a right-click) and choose
"Delete" option. You can hold down Shift key to select multiple
tracks in a row or a Ctrl key to select individual tracks.

To move tracks, select one or more of them, drag them and drop
where they're supposed to end up.

To start a new playlist, use the "File >> New playlist" option.
WARNING: It will clear the current playlist.

To save the current playlist, use the "File >> Save playlist"
option. You can then choose the saving location of the playlist.

To open an existing playlist, use the "File >> Load playlist"
option.

To change basic playlist properties (such as name or the saving
path), use the "File >> Manage playlist" option. You will need
to save the playlist for the changes to persist, and the playlist
path changes to the last location the playlist was saved.

There is also an option to make the playlist shareable or portable.
It is elaborated on in the further section.


Track management
----------------

To edit an individual track, select it, open its context menu
and choose "Edit" option. You can change track parameters from
there.

In the "Info" section you can change the name and the path
of the track. The name is what is displayed in the playlist,
while the path points to the audio file location. It can be
either the full filesystem path, or the path relative to
the containing playlist. Most of the time it's recommended to
use full paths.

In the "Playback" section you can change the looping parameters.
A loop is defined by four values: track beginning, loop beginning,
loop end and track end. The track range defines the section of
the original audio file to be played. The loop range defines
the points between which the soundtrack loops.

Thus, the specific track can be divided into three sections:

- intro, between the track beginning and the loop beginning
- loop, between the loop beginning and the loop end
- outro, between the loop end and the track end

It is also possible to define the number of loop repetitions
before the loop proceeds to outro. By default, the loop plays
indefinitely and the outro is never actually reached.

To change the looping parameters, the new values must be valid.
In particular, the order of points must be kept (first track
beginning, then loop beginning, then loop end, then trakc end)
and the loop must have non-zero length.


Playback
--------

To play a given track, double-click on its playlist entry.

You can control the playback by clicking Play, Pause and Stop
buttons. Also, you can change the volume by moving the volume
slider next to the stop button.

You can also change the position of the track by clicking on
the player bar. The darker areas of the player bar correspond
to the track intro and outro sections.



Additional remarks
==================

Portability and shareability
----------------------------

Typically, a playlist is either portable or shareable.
The difference between the two is related to their usage.

A portable playlist is meant to be used on a local filesystem.
It can be freely moved around and work from whichever directory
it has been opened. Under the hood, it uses absolute paths
for all main playlist items.

In contrast, a shareable playlist is meant to be shared around,
either posted on the Internet or included with a specific album.
It has no knowledge of the local filesystem, and thus must be
placed at a specific position (typically in the same directory
as the album tracks). It won't work from other place, because it
won't be able to find the correct audio files. Under the hood,
it uses relative paths for every single playlist item.

The playlist can be adjusted in the playlist management window,
available through the "File >> Manage playlist" option. You can
make the playlist shareable or portable by clicking corresponding
buttons; since this action cannot be reversed, a confirmation is
required.

If you got a Music Room playlist from the Internet, you might
want to open it from its appropriate location, make it portable
and save it in a more convenient place.

Conversely, if you prepare a playlist for a specific album and
decide to share it with everyone, you will need to make it
shareable beforehand, so that no filesystem-specific information
remains.


Convenient playlist opening
---------------------------

Usually, one would open the Music Room application and load
a playlist from there. However, it is possible to open playlist
files directly.

If *.mrpl files have no associated application, try to open any
playlist. Otherwise, open context menu for the file and choose
"Open with >> Choose default program" option.

Either way, a program selection window should appear. Use "Browse"
option and choose the Music Room executable, wherever it is.
Make sure the "Always use the selected program" option is checked.

After that, the system should make the association between
Music Room and its playlists. The playlists should be easily
recognisable by the Music Room icon as well.



Changelog
=========

0.2.0.0 - initial release



Useful links
============

Music Room official page (along with premade playlists):
http://software.alphish.com/music-room

Updates and new playlists information:
https://twitter.com/AlphishCreature

Music Room source repository:
https://github.com/Alphish/music-room



Enjoy~!
