Learn this Visual Novel script format and write a simple story.

```
# commands starting with __ is builtin functions, not allowed to be used by user

# System settings
-extern waitForClick # wait for a click
-extern wait time=1 # wait for a certain delay
-extern canSkip val # true or false(default)
-extern startAsync # the next commands will start simultaneously
-extern endAsync   # ends that

# Characters
-extern __charDefine id
-extern __charDelete id
-extern charSwitch id path # switch character image
-extern charMove id x=0.0 y=0.0 tween=0 time=0.2
-extern charAlpha id alpha=1.0 tween=0 time=0.2
-extern charScale id x=1.0 y=1.0 tween=0 time=0.2

-macro charSpawn id path x=0.0 y=0.0 sx=1.0 sy=1.0 alpha=1.0 tween=0 time=0.2 # spawn a character
@__charDefine id
@charSwitch id path
@startAsync
@charScale id sx sy tween time
@charMove id x y tween time
@charAlpha id alpha tween time
@endAsync

-macro charRemove id tween=0 time=0.2 # remove a character
@charAlpha id 0 tween time
@__charDelete id

# Audios
-extern bgm path volume=1.0 operation="play" # play or stop
-extern sfx path volume=1.0 operation="play"

# Background Image
-extern __bgSwitch path
-extern __bgRemove
-extern bgAlpha alpha=1.0 tween=0 time=0.2
-extern bgMove x=0 y=0 tween=0 time=0.2

-macro bgSwitch path tween=0 time=1 # switch background image, real time would be 2s !IMPORTANT
@bgAlpha 0 tween time
@__bgSwitch path
@startAsync
@bgAlpha 1 tween time
@bgMove 0 0 tween time
@endAsync

-macro bgRemove tween=0 time=0.2 # remove background to black
@bgAlpha 0 tween time
@bgMove 0 0 0 0
@__bgRemove


# E.G.
-scene "__entrypoint__" # define a scene to write scripts
@bgm "1.mp3" 1
@bgSwitch "BG1.png"
Hello, World!
@charSpawn "A" "A_001.png"
A: 你好！
@charSwitch "A" "A_002.png"
@startAsync
@charMove "A" 0.5 0 --time 1 --tween 11
@charScale "A" 1.1 1.1 --time 1 --tween 11
@charAlpha "A" 0.6 --time 1 --tween 11
@endAsync
@charMove "A" -0.5 0 --time 1 --tween 6
@charSwitch "A" "A_001.png"
A: 我可以左右乱动！
@charRemove "A"
@bgAlpha 0.5
A: 背景消失中。。。50%
@bgAlpha 0.25
A: 背景消失中。。。75%
@bgAlpha 0
A: 背景没啦！！！
@bgAlpha 0.6
A: 又回来了60%！
@bgSwitch "BG2.png"
A: 换了一张呢！
以上为示例脚本内容
```