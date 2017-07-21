#!/bin/bash

cd $(dirname "$0")

#./gradlew clean

./gradlew assembleRelease

mv proj.android/build/outputs/apk/proj.android-release.apk android-release.apk

