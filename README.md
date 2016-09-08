# Plak

Run a local webserver in Windows to use the copypaste clipboard in Bash on Ubuntu on Windows.

```
$ wpaste > ~/clipboard-contents
$ fortune | wcopy
```

## Installation in Windows

1. Press Windows + R, type `shell:startup`, and press Enter.
2. With the right mouse button, drag `Plak.Windows.exe` to your Startup folder and choose *Create shortcuts here*.
3. You may want to change the port number from the default (9001). To do so, press Alt + Enter and add a port number to Target, e.g. `C:\Plak.Windows.exe 8080`.
4. Launch the shortcut.

Note that `Plak` runs in the background. If you want to stop using Plak, use Task Manager to end the `Plak.Windows` task.

## Installation in Ubuntu

### Bash

Add to `~/.bashrc`:

```bash
PLAKPORT=9001
alias wcopy="curl -s -X PUT \
 -H 'Content-Type: text/plain; charset=utf-8' \
 http://localhost:${PLAKPORT}/clipboard \
 --data-binary @-"
alias wpaste="curl -s http://localhost:${PLAKPORT}/clipboard"
```

### Emacs

Add to your init file:

```elisp
(defvar plak-port 9001)
(defun wcopy ()
  "Copies with Plak"
  (interactive)
  (shell-command-on-region (region-beginning) (region-end)
    (concat "curl -s -X PUT "
            "-H 'Content-Type: text/plain; charset=utf-8' "
            "http://localhost:" (number-to-string plak-port) "/clipboard "
            "--data-binary @-")))
(defun wpaste ()
  "Pastes with Plak"
  (interactive)
  (insert (shell-command-to-string (concat "curl -s http://localhost:"
                                           (number-to-string 9001)
                                           "/clipboard"))))
```

Now you can use or keymap the `wcopy` and `wpaste` commands.

## License

Copyright © 2016 Sander Dijkhuis

Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following
conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
