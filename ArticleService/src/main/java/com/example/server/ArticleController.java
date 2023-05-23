package com.example.server;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.Optional;

@RestController
public class ArticleController {
    @RequestMapping("/home")
    public String serviceTest(){
        return "Der Service ist UP!";
    }

    @Autowired
    private ArticleService articleService;

    @RequestMapping("/articles")
    public List<Article> getallArticles() {
        return articleService.getallArticles();
    }


    @RequestMapping("/articles/{id}")
    public Optional<Article> getArticle(@PathVariable String id) {
        return articleService.getArticle(id);
    }


    @RequestMapping(method= RequestMethod.POST, value="/articles")
    public String addArticle(@RequestBody Article article) {
        articleService.addArticle(article);
        String response = "{\"success\": true, \"message\": Article has been added successfully.}";
        return response;
    }

    @RequestMapping(method = RequestMethod.PUT, value="/articles/{id}")
    public String updateArticle(@RequestBody Article article, @PathVariable String id) {
        articleService.updateArticle(id, article);
        String response = "{\"success\": true, \"message\": Article (ID: " + id + ") has been updated successfully.}";
        return response;
    }

    @RequestMapping(method = RequestMethod.DELETE, value="/articles/{id}")
    public String deleteArticle(@PathVariable String id) {
        articleService.deleteArticle(id);
        String response = "{\"success\": true, \"message\": Article (ID: " + id + ") has been deleted successfully.}";
        return response;
    }
}

