### ROADMAP

✔ TAB and SHIFT+TAB to exit / re-enter the tag tree filter

✔ visible highlighting for arrow key navigation of tag tree window

✔ ENTER to open highlighted tag in tag tree

✔ allow copying blocks between open tags without having to close and reopen them

✔ separate apply and save button actions on options page

✔ added startup mod cache option	

🔜 disable keyboard navigation to the info window until it's reimplemented  
--- it is useless in its current state and just steals focus

🔜 intuitive / standardized tab docking and navigation shortcuts

🔜 add Poke functionality from Epsilon-Poke application version  
--- already found the code and implementation in ILSpy

🔜 script window text editing (general improvements, shouldn't be garbage)

🔜 script window save button should also save the tag

🔜 script window compile errors should not be garbage

🔜 intelligent handling for multiple tabs  
(note to self: multiple edittag context for same tag)  
* if tag is already open, switch focus to it
* how can we know with certainty that a tag we're attempting to open is equal to one that's already open?
* add an option to open a "reference" tag - a snapshot / read-only version of the tag

🔜 show editing context stack per tag  
* or cache? idk exactly how the context works  
* hover a tag or a cache entry to see the stack  

🔜 arbitrary tag collections
- favorited / pinned tags
- named groups of tags
- open / close / reload / save / 'backup' all tags in a group
- persistent per-group setting to automatically open a group on startup  
(with optional dependency on a specific cache or mod package)

🔜 reimplement "info window" in a more sensible way
- make sure the info can actually be selected and copied
- it would probably make more sense as a pop-up info window triggered from a menu or right-click menu